// ---------------------------------------------------------------------------------------------
// <copyright file="OtpManager.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using Amazon.Runtime.Internal;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using SuperbackCiamOTP.Entities;
using SuperbackCiamOTP.Interfaces;

namespace SuperbackCiamOTP.Managers
{
    public class OtpManager : IOtpManager
    {
        private IRedisManager redisManager;
        private ISmsManager smsManager;
        private IWhatsAppManager whatsAppProviderManager;
        private ILogger logger;
        private IOtpResources resources;
        private IEmailManager emailManager;
        private ISerializador serializador;

        public OtpManager(
            IRedisManager redisManager,
            ISmsManager smsManager,
            IWhatsAppManager whatsAppProviderManager,
            ILogger<RedisManager> logger,
            IOtpResources resources,
            IEmailManager emailManager,
            ISerializador serializador)
        {
            this.redisManager = redisManager;
            this.smsManager = smsManager;
            this.whatsAppProviderManager = whatsAppProviderManager;
            this.logger = logger;
            this.resources = resources;
            this.emailManager = emailManager;
            this.serializador = serializador;
        }

        public async Task<bool> SendOtp(OtpModel otp)
        {
            bool isSave = false;
            string errorOtp;
            string infoOtp;
            ChannelType channelType;
            OtpType otpType;
            TemplateType templateType;
            if (otp.PhoneNumber.StartsWith("+1") && otp.Channel == "SMS")
            {
                this.logger.LogWarning("US and Canadian numbers should use Whatsapp channel to send phone messages");
                otp.Channel = "WHATSAPP";
            }

            GetEnums(otp, out channelType, out otpType);
            otp.Otp = this.GenerateOtp(otpType);
            infoOtp = string.Format(this.resources.ChanelTypeInfo(), channelType);
            this.logger.LogInformation(this.serializador.Serialize(infoOtp, new()));

            switch (channelType)
            {
                case ChannelType.EMAIL:
                    otp.PhoneNumber = string.Empty;
                    this.GetValueRedis(ref otp, otp.Email, otp.Type);
                    isSave = await this.redisManager.Saves("otpmodel:" + otp.Email, JsonSerializer.Serialize(otp), When.Always);
                    otp.TemplateId = TemplateIdConstants.SpinPlusEmailVerification;
                    if (Enum.TryParse<TemplateType>(otp.TemplateId, out templateType))
                    {
                        var responseEmail = await this.emailManager.SendEmailTemplate(otp.Email, otp.Otp, otp.TemplateId);
                    }
                    else
                    {
                        if (otp.TemplateId != TemplateIdConstants.SpinPlusOnboarding)
                        {
                            errorOtp = this.resources.SendOtpError();
                            this.logger.LogError(this.serializador.Serialize("errorOtp: " + errorOtp, new()));
                            throw new Exception(errorOtp);
                        }

                        var responseEmail = await this.emailManager.SendSimpleEmail(otp.Email, string.Format(TemplateConstants.SpinPlusOnboarding, otp.Otp));
                    }

                    break;
                case ChannelType.SMS:
                    if (otp.TemplateId != TemplateIdConstants.SpinPlusOnboarding)
                    {
                        errorOtp = this.resources.SendOtpError();
                        this.logger.LogError(this.serializador.Serialize("errorOtp: " + errorOtp, new()));
                        throw new Exception(errorOtp);
                    }

                    otp.Email = string.Empty;
                    this.GetValueRedis(ref otp, otp.PhoneNumber, otp.Type);
                    isSave = await this.redisManager.Saves("otpmodel:" + otp.Id, JsonSerializer.Serialize(otp), When.Always);
                    infoOtp = string.Format(this.resources.IsSaveRedisStatus(), isSave, otp.Channel);
                    this.logger.LogInformation(this.serializador.Serialize(infoOtp, new()));
                    var result = await this.smsManager.SingleSend(otp.PhoneNumber, string.Format(TemplateConstants.SpinPlusOnboarding, otp.Otp));
                    break;
                case ChannelType.WHATSAPP:
                    otp.Email = string.Empty;
                    otp.Message = string.Empty;
                    this.GetValueRedis(ref otp, otp.PhoneNumber, otp.Type);
                    var responseTemplate = await this.whatsAppProviderManager.CreateTemplate(otp.PhoneNumber, otp.TemplateId, otp.Otp);
                    if (responseTemplate.IsCreated)
                    {
                        this.GetValueRedis(ref otp, otp.PhoneNumber, otp.Type);
                        isSave = await this.redisManager.Saves("otpmodel:" + otp.Id, JsonSerializer.Serialize(otp), When.Always) && responseTemplate.IsCreated;
                        infoOtp = string.Format(this.resources.IsSaveRedisStatus(), isSave, otp.Channel);
                        this.logger.LogInformation(this.serializador.Serialize(infoOtp, new()));
                    }

                    break;
                default:
                    errorOtp = this.resources.SendOtpError();
                    this.logger.LogError(this.serializador.Serialize("errorOtp: " + errorOtp, new()));
                    throw new Exception(errorOtp);
            }

            return isSave;
        }

        private static void GetEnums(OtpModel otp, out ChannelType channelType, out OtpType otpType)
        {
            _ = Enum.TryParse<ChannelType>(otp.Channel, out channelType);
            _ = Enum.TryParse<OtpType>(otp.Type, out otpType);
        }

        public async Task<bool> DeleteOtp(string id)
        {
            var isSuccess = await this.redisManager.Delete(id);
            this.logger.LogInformation(this.serializador.Serialize($"Delete key {id}: {isSuccess}", new()));
            return isSuccess;
        }

        private string GenerateOtp(OtpType otpType)
        {
            switch (otpType)
            {
                case OtpType.NUMERIC:
                    var otp = RandomNumberGenerator.GetInt32(OtpConfiguration.MinLength, OtpConfiguration.MaxLength);
                    return otp.ToString();
                default:
                    var otpTypeNotSupported = this.resources.OtpTypeNotSupported();
                    this.logger.LogError(this.serializador.Serialize("otpTypeNotSupported: " + otpTypeNotSupported, new()));
                    throw new Exception(otpTypeNotSupported);
            }
        }

        private void GetValueRedis(ref OtpModel otp, string key, string type)
        {
            otp.Date = DateTime.UtcNow;
            otp.Id = key;
            otp.Expiration = OtpConfiguration.Expiration;
            otp.Type = type;
            otp.Message = otp.Message;
        }

        private async Task<bool> SetTriesCreateOtp(ValidationOtpModel valueRedis)
        {
            // Increase the OTP validation tries
            if (valueRedis.TriesCreateOtp == OtpConfiguration.MaxTriesCreateOtp)
            {
                // OTP validation tries reach its maximum, needs to retun to beginning
                valueRedis = this.SetTriesOnboarding(valueRedis);
                if (valueRedis.TriesOnboarding == OtpConfiguration.MaxTriesOnboardingFlow)
                {
                    // Reach maximum number of onboarding tries
                    _ = await this.SetMaxTriesOnboarding(valueRedis);
                    this.logger.LogError("User reach the limit of onboarding loops per session, it is now suspended");
                    throw new Exception("Reach maximum onboarding tries");
                }
                else
                {
                    // Needs to return to beginning
                    await this.redisManager.Delete("otpmodel:" + valueRedis.Id);
                    await this.redisManager.Saves("validationotp:" + valueRedis.Id, JsonSerializer.Serialize(valueRedis), When.Always);
                    this.logger.LogError("User reach the limit of OTP generation per session");
                    throw new Exception("Limit send tries reach");
                }
            }

            valueRedis.TriesCreateOtp += 1;
            valueRedis.DateTriesCreate = DateTime.UtcNow;
            await this.redisManager.Saves("validationotp:" + valueRedis.Id, JsonSerializer.Serialize(valueRedis), When.Always);
            return true;
        }

        private ValidationOtpModel SetTriesOnboarding(ValidationOtpModel valueRedis)
        {
            // Increase number of onboarding tries
            valueRedis.TriesOnboarding += 1;
            valueRedis.TriesCreateOtp = 0;
            valueRedis.TriesValidateOtp = 0;
            return valueRedis;
        }

        private async Task<bool> SetMaxTriesOnboarding(ValidationOtpModel valueRedis)
        {
            // Reach maximum number of onboarding tries
            // Block user for a day
            await this.redisManager.Delete("otpmodel:" + valueRedis.Id);
            await this.redisManager.Delete("validationotp:" + valueRedis.Id);
            valueRedis.TriesCreateOtp = valueRedis.TriesCreateOtp == OtpConfiguration.MaxTriesCreateOtp ? 0 : valueRedis.TriesCreateOtp;
            valueRedis.TriesValidateOtp = valueRedis.TriesValidateOtp == OtpConfiguration.MaxTriesValidateOtp ? 0 : valueRedis.TriesValidateOtp;
            valueRedis.DateBlocked = DateTime.UtcNow;
            valueRedis.IsBlocked = true;
            valueRedis.DateTriesOnboarding = DateTime.UtcNow;
            await this.redisManager.Saves("validationotp:" + valueRedis.Id, JsonSerializer.Serialize(valueRedis), When.Always);
            return false;
        }

        public async Task<bool> CheckValidationOtp(string identifier)
        {
            var valueRedis = this.redisManager.GetValue("validationotp:" + identifier);
            if (!valueRedis.Result.IsNull)
            {
                // Check if validation tries reach its maximum in last fail validation
                var model = JsonSerializer.Deserialize<ValidationOtpModel>(valueRedis.Result.ToString());
                if (model == null)
                {
                    model = new ValidationOtpModel();
                }

                if (model.TriesValidateOtp == OtpConfiguration.MaxTriesValidateOtp)
                {
                    // Reach maximum number of validation tries
                    model = this.SetTriesOnboarding(model);
                    if (model.TriesOnboarding == OtpConfiguration.MaxTriesOnboardingFlow)
                    {
                        // Reach maximum number of onboarding tries
                        _ = await this.SetMaxTriesOnboarding(model);
                        this.logger.LogError("User reach the limit of onboarding loops per session, it is now suspended");
                        throw new Exception("Limit validation tries reach");
                    }
                    else
                    {
                        // Needs to return to start page
                        await this.redisManager.Delete("otpmodel:" + model.Id);
                        await this.redisManager.Saves("validationotp:" + model.Id, JsonSerializer.Serialize(model), When.Always);
                        this.logger.LogError("User reach the limit of OTP validations per session");
                        throw new Exception("Limit validation tries reach");
                    }
                }
            }

            // Everything ok
            return true;
        }

        private async Task<bool> SetTriesValidateOtp(ValidationOtpModel valueRedis)
        {
            valueRedis.TriesValidateOtp += 1;
            valueRedis.DateValidateOtp = DateTime.UtcNow;
            await this.redisManager.Saves("validationotp:" + valueRedis.Id, JsonSerializer.Serialize(valueRedis), When.Always);
            return true;
        }

        private bool IsValidRequest(ValidationOtpModel valueRedis, DateTime date)
        {
            var diffInSeconds = (DateTime.UtcNow - TimeZoneInfo.ConvertTimeToUtc(date)).TotalSeconds;
            this.logger.LogInformation(this.serializador.Serialize($"diffInSeconds between request: {diffInSeconds}", new()));
            if (diffInSeconds <= OtpConfiguration.SecondBetweenRequest || valueRedis.IsBlocked)
            {
                return false;
            }

            return true;
        }

        private bool IsValidOtp(OtpModel valueRedis)
        {
            return (valueRedis.Date <= DateTime.UtcNow) && ((DateTime.UtcNow - valueRedis.Date).TotalSeconds <= valueRedis.Expiration);
        }

        public async Task<bool> CouldGenerateOtp(string identifier)
        {
            var valueRedis = this.redisManager.GetValue("validationotp:" + identifier);
            ValidationOtpModel model = new ValidationOtpModel();
            if (!valueRedis.Result.IsNull)
            {
                model = JsonSerializer.Deserialize<ValidationOtpModel>(valueRedis.Result.ToString());
            }

            if (model.Id.Equals(string.Empty))
            {
                // First time to generate OTP
                ValidationOtpModel models = new ValidationOtpModel()
                {
                    Id = identifier ?? string.Empty,
                    DateTriesCreate = DateTime.UtcNow,
                    IsBlocked = false,
                };

                return await this.SetTriesCreateOtp(models);
            }
            else if (this.IsValidRequest(model, model.DateTriesCreate))
            {
                this.logger.LogInformation(this.serializador.Serialize($"IsValidRequest", new()));
                if (model.TriesCreateOtp <= OtpConfiguration.MaxTriesCreateOtp)
                {
                    return await this.SetTriesCreateOtp(model);
                }
                else
                {
                    // User is suspended, it cannot create more OTPs
                    this.logger.LogError("User tries to create a OTP but is suspended");
                    throw new Exception("User suspended");
                }
            }
            else if (model.IsBlocked)
            {
                // User is suspended, it cannot create more OTPs
                this.logger.LogError("User tries to create a OTP but is suspended");
                throw new Exception("User suspended");
            }
            else
            {
                this.logger.LogError("User tries to create a OTP but is temporal suspended");
                throw new Exception("Temporal suspension for spam retries");
            }
        }

        public async Task<bool> CouldValidateOtp(string identifier)
        {
            var valueRedis = this.redisManager.GetValue("validationotp:" + identifier);
            var model = new ValidationOtpModel();
            if (!valueRedis.Result.IsNull)
            {
                model = JsonSerializer.Deserialize<ValidationOtpModel>(valueRedis.Result.ToString());
            }

            if (model.Id.Equals(string.Empty))
            {
                return await this.SetTriesValidateOtp(new ValidationOtpModel()
                {
                    Id = identifier,
                    DateValidateOtp = DateTime.UtcNow,
                    IsBlocked = false,
                });
            }
            else if (model.IsBlocked)
            {
                // User is suspended, it cannot create more OTPs
                this.logger.LogError("User tries to create a OTP but is suspended");
                throw new Exception("User suspended");
            }
            else
            {
                if (model.TriesValidateOtp < OtpConfiguration.MaxTriesValidateOtp)
                {
                    return await this.SetTriesValidateOtp(model);
                }
                else
                {
                    _ = await this.CheckValidationOtp(identifier);

                    // User is suspended, it cannot create more OTPs
                    this.logger.LogError("User tries to create a OTP but is suspended");
                    throw new Exception("User suspended");
                }
            }
        }

        public static string GetToken(RequestJwt request)
        {
            return TokenManager.GetInternalToken(request);
        }

        public async Task<bool> ValidateOtp(string identifier, ValidateRequest request)
        {
            if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(request.Otp))
            {
                return false;
            }

            List<string> validOtps = new List<string>()
            {
                "1234",
                "1809",
                "6689",
            };

            var identifierValue = this.redisManager.GetValue("mockotp:" + identifier);
            var allowIdentifier = false;
            var identifierData = new OtpMockModel();
            if (!identifierValue.Result.IsNull)
            {
                identifierData = JsonSerializer.Deserialize<OtpMockModel>(identifierValue.Result.ToString());
                if (identifierData != null && identifierData.Id == identifier)
                {
                    allowIdentifier = true;
                }
            }

            if (validOtps.Contains(request.Otp) && (OtpConfiguration.OTPMockProves || allowIdentifier))
            {
                this.logger.LogWarning("The user is validated by mock OTP");
                await this.redisManager.Delete("otpmodel:" + identifier);
                await this.redisManager.Delete("validationotp:" + identifier);
                return true;
            }

            this.logger.LogInformation(this.serializador.Serialize($"identifier: {identifier}", new()));
            var valueRedis = this.redisManager.GetValue("otpmodel:" + identifier);
            OtpModel model = new OtpModel();
            if (!valueRedis.Result.IsNull)
            {
                model = JsonSerializer.Deserialize<OtpModel>(valueRedis.Result.ToString());
            }

            if (this.IsValidOtp(model) && request.Otp.Equals(model.Otp))
            {
                await this.redisManager.Delete("otpmodel:" + identifier);
                await this.redisManager.Delete("validationotp:" + identifier);
                this.logger.LogInformation(this.serializador.Serialize($"OTP VERIFICATION SUCCESS", new()));
                this.logger.LogInformation(this.serializador.Serialize("TOKEN OTP: " + model.Otp, new()));
                return true;
            }

            return false;
        }

        public string GetIdentifier(SendOtpRequest request)
        {
            ChannelType channelType;
            _ = Enum.TryParse<ChannelType>(request.Channel, out channelType);
            string identifier = string.Empty;
            switch (channelType)
            {
                case ChannelType.SMS:
                    identifier = request.PhoneNumber ?? string.Empty;
                    break;
                case ChannelType.WHATSAPP:
                    identifier = request.PhoneNumber ?? string.Empty;
                    break;
                case ChannelType.EMAIL:
                    identifier = request.Email ?? string.Empty;
                    break;
            }

            return identifier;
        }

        public async Task<SettingResponse> ReadAppSettings()
        {
            SettingResponse objSett = new SettingResponse() { Setting1 = OtpConfiguration.ReadApp1, Setting2 = OtpConfiguration.ReadApp2, Setting3 = OtpConfiguration.ReadApp3 };

            this.logger.LogInformation("consulta de appsettings");
            return objSett;
        }
    }
}
