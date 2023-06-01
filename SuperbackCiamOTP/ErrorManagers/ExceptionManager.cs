// ---------------------------------------------------------------------------------------------
// <copyright file="ExceptionManager.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using SuperbackCIAM.Middleware;
using SuperbackCiamOTP.Interfaces;

namespace SuperbackCIAM.Middleware
{
    public class ExceptionManager
    {
        private readonly RequestDelegate next;
        private readonly IHostEnvironment environment;
        private readonly ILogger<ExceptionManager> logger;
        private readonly IMiddlewareResource resource;

        public ExceptionManager(RequestDelegate next, IHostEnvironment environment, ILogger<ExceptionManager> logger, IMiddlewareResource resource)
        {
            this.next = next;
            this.environment = environment;
            this.logger = logger;
            this.resource = resource;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await this.next(httpContext);
            }
            catch (Exception ex)
            {
                var errorCode = ErrorCodes.GetError(ex.Message);
                httpContext.Response.StatusCode = (int)errorCode.StatusCodes;
                var middlewareException = string.Format(this.resource.MiddlewareException(), httpContext.Response.StatusCode, ex.Message, this.environment.IsDevelopment());
                this.logger.LogError(middlewareException);
                await HandleGlobalExceptionAsync(errorCode, httpContext, ex, this.environment.IsDevelopment(), this.logger);
            }
        }

        private static Task HandleGlobalExceptionAsync(Error error, HttpContext context, Exception exception, bool isDevelopment, ILogger<ExceptionManager> logger)
        {
            context.Response.ContentType = "application/json";
            ErrorDetails responseError;
            responseError = new ErrorDetails(
                errorCode: error.Code,
                message: isDevelopment && error.StatusCodes == HttpStatusCode.InternalServerError ? exception.Message : error.Message,
                detail: exception.Message);

            context.Response.StatusCode = (int)error.StatusCodes;

            logger.LogError(JsonConvert.SerializeObject(responseError));

            return context.Response.WriteAsync(JsonConvert.SerializeObject(responseError));
        }
    }
}
