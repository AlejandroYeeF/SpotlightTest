using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace SwaggerDocsDemo.Controllers
{
    /// <summary>
    /// Weather Forecasts
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [SwaggerTag("Get Weather forecast and place orders. Very weird and unstructed API :)")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        /// <summary>
        /// Constructor for Dependency Injection
        /// </summary>
        /// <param name="logger"></param>
        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// Return 3 random weather forecasts
        /// </summary>
        /// <remarks>
        /// This endpoint will return 3 days of weather forecasts with random temperatures in celcius.
        /// </remarks>
        /// <returns>5 Weather forecasts</returns>
        /// <response code="200">Returns the weather forecasts</response>
        [HttpGet(Name = "GetWeatherForecast")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}