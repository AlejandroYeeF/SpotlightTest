// ---------------------------------------------------------------------------------------------
// <copyright file="HealthController.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;

namespace SuperbackCiamOTP.Controllers
{
    [ApiController]
    [Route("otp/api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult Health()
        {
            return this.Ok("0.1.5");
        }
    }
}
