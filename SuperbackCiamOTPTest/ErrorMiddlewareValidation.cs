// ---------------------------------------------------------------------------------------------
// <copyright file="ErrorMiddlewareValidation.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using SuperbackCIAM.Middleware;
using SuperbackCiamOTP.Managers;

namespace SuperbackCiamOTPTest
{
    public class ErrorMiddlewareValidation
    {
        private readonly IConfiguration config;

        public ErrorMiddlewareValidation()
        {
            this.config = new ConfigurationDummy();
            ErrorCodes.Start(this.config);
        }

        [Fact]
        public void AddErrorsTest()
        {
            ErrorCodes.Start(this.config);
            ErrorCodes.AddError(
                new List<string>(),
                HttpStatusCode.OK,
                "SPCI-Test",
                "CIAM",
                "Adding a new error");
        }

        [Fact]
        public void GetErrorsTest()
        {
            var errorToken = ErrorCodes.GetError("User suspended");
            Assert.Equal("SPOP-403-3", errorToken.Code);
            var errorDefault = ErrorCodes.DefaultError();
            Assert.Equal("SPOP-500", errorDefault.Code);
        }

        [Theory]
        [InlineData("User suspended")]
        [InlineData("SMS Error")]
        public async void ExceptionManagerTest(string message)
        {
            // Create exception manager
            var environmentMock = new Mock<IHostEnvironment>();
            var loggerMock = new Mock<ILogger<ExceptionManager>>();
            var manager = new ExceptionManager(
                this.GenerateError(message),
                environmentMock.Object,
                loggerMock.Object,
                new MiddlewareResource());

            // User manager
            var context = new DefaultHttpContext();
            await manager.InvokeAsync(context);
        }

        private RequestDelegate GenerateError(string message)
        {
            return ctx =>
            {
                throw new Exception(message);
            };
        }

        private class ConfigurationDummy : IConfiguration
        {
            public string? this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public IEnumerable<IConfigurationSection> GetChildren()
            {
                throw new NotImplementedException();
            }

            public IChangeToken GetReloadToken()
            {
                throw new NotImplementedException();
            }

            public IConfigurationSection GetSection(string key)
            {
                throw new NotImplementedException();
            }
        }
    }
}
