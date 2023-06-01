// ---------------------------------------------------------------------------------------------
// <copyright file="OTPController.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SuperbackCIAM.Middleware;
using SuperbackCiamOTP.Entities;
using SuperbackCiamOTP.Interfaces;
using SuperbackCiamOTP.Managers;
using SuperbackCiamOTP.Validations;

namespace SuperbackCiamOTP.Controllers
{
    [ApiController]
    [Route("otp/api/[controller]/[action]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class OTPController : ControllerBase
    {
        private readonly IOtpManager otp;
        private readonly ILogger<OTPController> logger;
        private readonly IOtpControllerResources resources;
        private readonly ISerializador serializador;

        public OTPController(IOtpManager otp, ILogger<OTPController> logger, IOtpControllerResources resources, ISerializador serializador)
        {
            this.otp = otp;
            this.logger = logger;
            this.resources = resources;
            this.serializador = serializador;
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(SendOtpRequest))]
        public async Task<IActionResult> Send([FromBody] SendOtpRequest request)
        {
            this.logger.LogInformation(this.serializador.Serialize($"Send Request: ", request));
            if (await this.otp.CouldGenerateOtp(this.otp.GetIdentifier(request)))
            {
                this.logger.LogInformation(this.serializador.Serialize($"Generate OTP Request:", request));
                var isSend = await this.otp.SendOtp(new OtpModel
                {
                    PhoneNumber = request.PhoneNumber ?? string.Empty,
                    Email = request.Email ?? string.Empty,
                    Channel = request.Channel ?? string.Empty,
                    Type = request.Type ?? OtpType.NUMERIC.ToString(),
                    CodeService = request.ServiceCode ?? string.Empty,
                    Message = request.Message ?? string.Empty,
                    TemplateId = request.TemplateId ?? string.Empty,
                });
                this.logger.LogInformation(this.serializador.Serialize($"SendOtp:{isSend}", new()));
                this.logger.LogInformation($"CouldGenerateOtp: true");
                return this.Ok(request);
            }

            this.logger.LogInformation($"CouldGenerateOtp: false");
            return this.Ok(request);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ValidOtpResponse))]
        [ProducesResponseType(400, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> Validate([FromBody] ValidateRequest request)
        {
            string source = !string.IsNullOrWhiteSpace(request.PhoneNumber) ? request.PhoneNumber : request.Email ?? string.Empty;
            string validSource = string.Empty;
            if (request.ServiceCode == "EmailVerification") 
            {
                source = request.Email ?? string.Empty;
            }

            // Try validate phone number or email
            this.logger.LogInformation(this.serializador.Serialize($"Validate Request: ", request));

            try
            {
                if (await this.ValidateIdentifier(request, identifier: source))
                {
                    validSource = source;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(this.serializador.Serialize($"The otp validation failed:", ex));
                throw;
            }

            if (!string.IsNullOrWhiteSpace(validSource))
            {
                // Delete validation metadata generated for all sources
                await this.otp.DeleteOtp("otpmodel:" + source);
                await this.otp.DeleteOtp("validationotp:" + source);

                // Success validation
                string jwt = OtpManager.GetToken(
                    new RequestJwt()
                    {
                        Username = validSource,
                        ServiceCode = request.ServiceCode,
                    });
                this.logger.LogInformation(this.serializador.Serialize("AccessToken:", jwt));
                return this.Ok(new ValidOtpResponse() { AccessToken = jwt });
            }

            string unauthorizedDescription = this.resources.Unauthorized();
            this.logger.LogError($"unauthorized: {unauthorizedDescription}");
            throw new Exception("The otp validation failed.");
        }

        private async Task<bool> ValidateIdentifier(ValidateRequest request, string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                // Identifier is empty
                return false;
            }

            if (await this.otp.CouldValidateOtp(identifier))
            {
                // Exists identifier
                this.logger.LogInformation("CouldValidateOtp: true");
                if (!await this.otp.ValidateOtp(identifier, request))
                {
                    // Validation fails
                    // Check validation status
                    _ = await this.otp.CheckValidationOtp(identifier);
                    return false;
                }

                // Validation success
                return true;
            }
            else
            {
                this.logger.LogError("CouldValidateOtp: false");
                return false;
            }
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ValidOtpResponse))]
        public async Task<IActionResult> ReadAppSettings()
        {
            var userAPP = await this.otp.ReadAppSettings();
            this.logger.LogInformation(this.serializador.Serialize("ReadAppSettings Response: ", userAPP));
            return this.Ok(userAPP);
        }
    }
}
