// ---------------------------------------------------------------------------------------------
// <copyright file="Utilities.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;

namespace SuperbackCiamOTPTest
{
    public class Utilities
    {
        public static TypeResponse ProcessOkActionResult<TypeResponse>(IActionResult? actionResult)
        {
            // Get OkObjectResult
            var okResult = actionResult as OkObjectResult;

            // Verify it is not null
            Assert.NotNull(okResult);

            // Cast to respective TypeResponse
            return Assert.IsType<TypeResponse>(okResult.Value);
        }
    }
}
