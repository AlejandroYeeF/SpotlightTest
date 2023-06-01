// ---------------------------------------------------------------------------------------------
// <copyright file="HealthValidation.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using SuperbackCiamOTP.Controllers;

namespace SuperbackCiamOTPTest
{
    public class HealthValidation
    {
        [Fact]
        public void OkTest()
        {
            // Create health controller
            var healthController = new HealthController();

            // Use health
            var actionResult = healthController.Health();

            // Check action result
            var okObject = actionResult as OkObjectResult;

            // Check status code
            Assert.NotNull(okObject);
            Assert.Equal(200, okObject.StatusCode);
        }
    }
}
