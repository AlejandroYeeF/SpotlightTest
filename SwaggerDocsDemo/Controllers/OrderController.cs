using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace SwaggerDocsDemo.Controllers
{
    /// <summary>
    /// Orders
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [SwaggerTag("Get and insert orders")]
    public class OrderController : ControllerBase
    {
        public OrderController()
        {
        }

        /// <summary>
        /// Return all orders
        /// </summary>
        /// <remarks>
        /// This endpoint will return all orders.
        /// </remarks>
        /// <returns>All orders</returns>
        /// <response code="200">Returns all orders</response>
        [HttpGet("GetOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<Order>> GetOrders()
        {
            List<Order> orders = new List<Order>();
            return StatusCode(StatusCodes.Status200OK, orders);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost("AddOrder")]
        [SwaggerOperation(
            Summary = "Add a new order to the API",
            Description = "This endpoint will take in a new order and return it to the client.",
            OperationId = "AddOrder")]
        [SwaggerResponse(200, "The posted order payload", type: typeof(Order))]
        public ActionResult<Order> AddOrder([FromBody, SwaggerRequestBody("The order payload", Required = true)] Order order)
        {
            return StatusCode(StatusCodes.Status200OK, order);
        }
    }
}