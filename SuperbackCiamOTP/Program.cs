// ---------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Amazon.SimpleEmail;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;
using StackExchange.Redis;
using SuperbackCIAM.Middleware;
using SuperbackCiamOTP.Entities;
using SuperbackCiamOTP.Interfaces;
using SuperbackCiamOTP.Managers;

internal class Program
{
    private static void Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args).Build();

        var builder = WebApplication.CreateBuilder(args);

        // add initial configuration
        IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

        var currentEnvironment = builder.Environment;

        OtpConfiguration.GetInitialConfiguration(config);

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
        .CreateLogger();

        // config serilog
        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

        builder.Host.UseSerilog((context, services, configuration) => configuration.ReadFrom.Configuration(context.Configuration)
                                                                                   .ReadFrom.Services(services)
                                                                                   .Enrich.FromLogContext()
                                                                                   .WriteTo.Console(formatter: new JsonFormatter(renderMessage: true)));

        StaticLoggerFactory.Initialize(host.Services.GetRequiredService<ILoggerFactory>());

        OtpConfiguration.AddResources(builder.Services);

        OtpConfiguration.AddSingleton(builder.Services);
        OtpConfiguration.AddSmsApi(config, builder.Services);
        OtpConfiguration.AddWhatsAppApi(config, builder.Services);
        OtpConfiguration.AddAwsServices(config, builder.Services);
        builder.Services.AddSingleton<ISerializador, Serializador>();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        // Add services to the container.
        builder.Services.AddControllers();

        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(c =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var filePath = Path.Combine(System.AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(filePath);
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        ErrorCodes.Start(builder.Configuration);
        app.ConfigureCustomException();

        app.Run();
    }
}
