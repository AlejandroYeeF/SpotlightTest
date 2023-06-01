// ---------------------------------------------------------------------------------------------
// <copyright file="OtpControllerValidation.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System.Net;
using System.Text.Json;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using SuperbackCiamOTP.Controllers;
using SuperbackCiamOTP.Entities;
using SuperbackCiamOTP.Managers;
using SuperbackCiamOTP.Resources;
using SuperbackCiamOTP.Validations;

namespace SuperbackCiamOTPTest
{
    public class OtpControllerValidation
    {
        private readonly FakeDatabase fakeDatabase;
        private readonly HttpClient httpClientFake;
        private readonly Mock<ILogger<OtpManager>> loggerOtpManagerMock;
        private readonly Mock<ILogger<ValidateRequest>> loggerValidateRequestMock;
        private readonly Mock<ILogger<WhatsAppManager>> loggerWhatsAppManagerMock;
        private readonly Mock<ILogger<RedisManager>> loggerRedisManagerMock;
        private readonly Mock<ILogger<OTPController>> loggerOTPControllerMock;
        private readonly Mock<ILogger<EmailManager>> loggerEmailManagerMock;
        private readonly Mock<IAmazonSimpleEmailService> amazonSimpleEmailServiceMock;
        private readonly Mock<ILoggerFactory> loggerFactoryMock;

        public OtpControllerValidation()
        {
            // Mock
            this.fakeDatabase = new FakeDatabase();
            this.loggerOtpManagerMock = new Mock<ILogger<OtpManager>>();
            this.loggerValidateRequestMock = new Mock<ILogger<ValidateRequest>>();
            this.loggerWhatsAppManagerMock = new Mock<ILogger<WhatsAppManager>>();
            this.loggerRedisManagerMock = new Mock<ILogger<RedisManager>>();
            this.loggerOTPControllerMock = new Mock<ILogger<OTPController>>();
            this.loggerEmailManagerMock = new Mock<ILogger<EmailManager>>();
            this.amazonSimpleEmailServiceMock = new Mock<IAmazonSimpleEmailService>();
            this.loggerFactoryMock = new Mock<ILoggerFactory> { DefaultValue = DefaultValue.Mock };

            // Configuration
            this.SetDummyConfiguration();

            // Setup general mocks
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.Created,
                    Content = new StringContent("[{'id':1,'value':'1'}]"),
                })
                .Verifiable();
            this.httpClientFake = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://spinplus.com"),
            };

            this.amazonSimpleEmailServiceMock.Setup(
                x => x.SendTemplatedEmailAsync(
                    It.IsAny<SendTemplatedEmailRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    return new SendTemplatedEmailResponse();
                });
        }

        private void SetDummyConfiguration()
        {
            OtpConfiguration.SecretKey = "this is my custom Secret key for authentication";
            OtpConfiguration.ExpiresToken = "86400";
            OtpConfiguration.Issuer = "otp";
            OtpConfiguration.Audience = "oxxo";
            OtpConfiguration.SecondBetweenRequest = 0;

            SmsConfig.Uri = "https://api.ondemand.com";
            SmsConfig.RequestUri = "controller";
            SmsConfig.Encpwd = "1234";
            SmsConfig.Cte = "1234";
            SmsConfig.Email = "luis@digitalfemsa.com";

            WhatsAppConfig.ApiKey = "api-key";
            WhatsAppConfig.Uri = "https://api.auronix.com";
            WhatsAppConfig.RequestUri = "transaction";
            WhatsAppConfig.Channel = "1234";
            WhatsAppConfig.DestinationUserId = "spinPremiaPlus";
            WhatsAppConfig.TemplateId = "c1a52602_8687_4104_97bd_71f593ee353c:spin_plus_otp_01";
            WhatsAppConfig.Language = "es_MX";
            WhatsAppConfig.BlackListIds = new string[] { };
            WhatsAppConfig.TransationMetadata = new List<TransationMetadata>();

            EmailConfig.AccessKeyId = "1234";
            EmailConfig.SecretKeyId = "4321";
            EmailConfig.RegionEndpoint = "us-east-1";
            EmailConfig.Subject = "Verification Otp";
            EmailConfig.From = "luis@digitalfemsa.com";
            EmailConfig.Source = "luis@digitalfemsa.com";

            try
            {
                StaticLoggerFactory.Initialize(this.loggerFactoryMock.Object);
            }
            catch (Exception ex)
            {
                // It is expected to have the error "StaticLogger already initialized!" on unit tests
                Assert.Equal("StaticLogger already initialized!", ex.Message);
            }
        }

        [Theory]
        [InlineData("SMS")]
        [InlineData("WHATSAPP")]
        public async void SendPhoneOtpTest(string channel)
        {
            // Arrange
            var username = "+521234567890";

            // Act
            var otpController = this.CreateOtpController();
            var createResult = await otpController.Send(
                new SendOtpRequest
                {
                    PhoneNumber = username,
                    Channel = channel,
                    TemplateId = TemplateIdConstants.SpinPlusOnboarding,
                });
            var createResponse = Utilities.ProcessOkActionResult<SendOtpRequest>(createResult);
            var otpModel = this.fakeDatabase.GetOtpModel(username);

            // Assert
            Assert.Equal(username, createResponse.PhoneNumber);
            Assert.Equal(channel, createResponse.Channel);
            Assert.Equal(username, otpModel.PhoneNumber);
            Assert.Equal(channel, otpModel.Channel);
        }

        [Fact]
        public async void SendEmailOtpTest()
        {
            // Arrange
            var username = "abc@email.com";
            var channel = "EMAIL";

            // Act
            var otpController = this.CreateOtpController();
            var createResult = await otpController.Send(
                new SendOtpRequest
                {
                    Email = username,
                    Channel = channel,
                    TemplateId = TemplateIdConstants.SpinPlusOnboarding,
                });
            var createResponse = Utilities.ProcessOkActionResult<SendOtpRequest>(createResult);
            var otpModel = this.fakeDatabase.GetOtpModel(username);

            // Assert
            Assert.Equal(username, createResponse.Email);
            Assert.Equal(channel, createResponse.Channel);
            Assert.Equal(username, otpModel.Email);
            Assert.Equal(channel, otpModel.Channel);
        }

        [Fact]
        public async void ValidateOtpTest()
        {
            // Arrange
            var username = "+521234567890";
            var channel = "SMS";

            // Act
            var otpController = this.CreateOtpController();

            // Any value is invalid
            await Assert.ThrowsAsync<Exception>(
                () => otpController.Validate(
                new ValidateRequest
                {
                    PhoneNumber = username,
                    Otp = "1234",
                    ServiceCode = "Onboarding",
                }));

            // Send OTP
            var createResult = await otpController.Send(
                new SendOtpRequest
                {
                    PhoneNumber = username,
                    Channel = channel,
                    TemplateId = TemplateIdConstants.SpinPlusOnboarding,
                });
            var createResponse = Utilities.ProcessOkActionResult<SendOtpRequest>(createResult);

            // Validate OTP
            var otpModel = this.fakeDatabase.GetOtpModel(username);
            var validateResult = await otpController.Validate(
                new ValidateRequest
                {
                    PhoneNumber = username,
                    Otp = otpModel.Otp,
                    ServiceCode = "Onboarding",
                });
            var validateResponse = Utilities.ProcessOkActionResult<ValidOtpResponse>(validateResult);
            Assert.True(!string.IsNullOrWhiteSpace(validateResponse.AccessToken));
        }

        [Fact]
        public async void ValidateOtpLastAttemptTest()
        {
            // Arrange
            var username = "+521234567890";
            var channel = "SMS";

            // Act
            var otpController = this.CreateOtpController();

            // Send OTP
            await otpController.Send(
                new SendOtpRequest
                {
                    PhoneNumber = username,
                    Channel = channel,
                    TemplateId = TemplateIdConstants.SpinPlusOnboarding,
                });

            var otpModel = this.fakeDatabase.GetOtpModel(username);
            var correctOtp = otpModel.Otp ?? "1234";
            var wrongOtp = (correctOtp[0] == '0') ? correctOtp.Replace('0', '1') : correctOtp.Replace(correctOtp[0], '0');
            for (int i = 1; i < OtpConfiguration.MaxTriesValidateOtp; ++i)
            {
                await Assert.ThrowsAsync<Exception>(
                    () => otpController.Validate(
                    new ValidateRequest
                    {
                        PhoneNumber = username,
                        Otp = wrongOtp,
                        ServiceCode = "Onboarding",
                    }));
            }

            // Validate OTP
            var validateResult = await otpController.Validate(
                new ValidateRequest
                {
                    PhoneNumber = username,
                    Otp = correctOtp,
                    ServiceCode = "Onboarding",
                });
            var validateResponse = Utilities.ProcessOkActionResult<ValidOtpResponse>(validateResult);
            Assert.True(!string.IsNullOrWhiteSpace(validateResponse.AccessToken));
        }

        [Fact]
        public async void DelayBetweenSendOtpTest()
        {
            // Arrange
            OtpConfiguration.SecondBetweenRequest = 1000000;
            var username = "abc@email.com";
            var secondUsername = "abc@correo.com";
            var channel = "EMAIL";

            // Act
            // Call to different usernames is allowed
            var otpController = this.CreateOtpController();
            var createResult = await otpController.Send(
                new SendOtpRequest
                {
                    Email = username,
                    Channel = channel,
                    TemplateId = TemplateIdConstants.SpinPlusOnboarding,
                });
            var createResponse = Utilities.ProcessOkActionResult<SendOtpRequest>(createResult);
            createResult = await otpController.Send(
                new SendOtpRequest
                {
                    Email = secondUsername,
                    Channel = channel,
                    TemplateId = TemplateIdConstants.SpinPlusOnboarding,
                });
            createResponse = Utilities.ProcessOkActionResult<SendOtpRequest>(createResult);

            try
            {
                // Call to the same username in less than 'SecondBetweenRequest' is not allowed
                await otpController.Send(
                    new SendOtpRequest
                    {
                        Email = username,
                        Channel = channel,
                        TemplateId = TemplateIdConstants.SpinPlusOnboarding,
                    });
                Assert.True(false, "Checking delay between calls - should fail second call");
            }
            catch (Exception ex)
            {
                Assert.Equal("Temporal suspension for spam retries", ex.Message);
            }

            OtpConfiguration.SecondBetweenRequest = 0;
        }

        [Fact]
        public async void ValidateMockProvesOtpTest()
        {
            var otpController = this.CreateOtpController();
            OtpConfiguration.OTPMockProves = true;
            await otpController.Validate(
                new ValidateRequest
                {
                    PhoneNumber = "+521234567890",
                    Otp = "1234",
                    ServiceCode = "Onboarding",
                });
            OtpConfiguration.OTPMockProves = false;
        }

        [Fact]
        public async void USOutcoverageSMSTest()
        {
            // Arrange
            // Canada or US phone number
            var username = "+11234567890";
            var smsChannel = "SMS";
            var whatsappChannel = "WHATSAPP";

            // Act
            var otpController = this.CreateOtpController();

            // Send via SMS
            var createResult = await otpController.Send(
                new SendOtpRequest
                {
                    PhoneNumber = username,
                    Channel = smsChannel,
                    TemplateId = TemplateIdConstants.SpinPlusOnboarding,
                });
            var createResponse = Utilities.ProcessOkActionResult<SendOtpRequest>(createResult);
            var otpModel = this.fakeDatabase.GetOtpModel(username);

            // Assert
            Assert.Equal(username, createResponse.PhoneNumber);
            Assert.Equal(smsChannel, createResponse.Channel);

            // Send as Whatsapp
            Assert.Equal(whatsappChannel, otpModel.Channel);
        }

        [Fact]
        public async void ExceedGenerateOtpTest()
        {
            // Arrange
            var username = "+521234567890";
            var channel = "SMS";

            // Act
            var otpController = this.CreateOtpController();
            var prevValidationModel = this.fakeDatabase.GetValidationOtp(username);

            for (int i = 0; i < OtpConfiguration.MaxTriesCreateOtp; ++i)
            {
                // Send OTP
                var createResult = await otpController.Send(
                    new SendOtpRequest
                    {
                        PhoneNumber = username,
                        Channel = channel,
                        TemplateId = TemplateIdConstants.SpinPlusOnboarding,
                    });
                var createResponse = Utilities.ProcessOkActionResult<SendOtpRequest>(createResult);
            }

            // Send extra OTP
            try
            {
                await otpController.Send(
                new SendOtpRequest
                {
                    PhoneNumber = username,
                    Channel = channel,
                    TemplateId = TemplateIdConstants.SpinPlusOnboarding,
                });
                Assert.True(false, "It is expected to fail the send otp");
            }
            catch (Exception ex)
            {
                Assert.Equal("Limit send tries reach", ex.Message);
            }

            var validationModel = this.fakeDatabase.GetValidationOtp(username);

            // Assert
            Assert.Equal(3, OtpConfiguration.MaxTriesCreateOtp);
            Assert.Equal(0, prevValidationModel.TriesOnboarding);
            Assert.Equal(1, validationModel.TriesOnboarding);
        }

        [Fact]
        public async void ExceedValidateOtpTest()
        {
            // Arrange
            var username = "+521234567890";

            // Act
            var otpController = this.CreateOtpController();
            for (int i = 0; i < OtpConfiguration.MaxTriesValidateOtp - 1; ++i)
            {
                // Validate OTP
                try
                {
                    await otpController.Validate(
                        new ValidateRequest
                        {
                            PhoneNumber = username,
                            Otp = "1234",
                            ServiceCode = "Onboarding",
                        });
                }
                catch (Exception ex)
                {
                    Assert.Equal("The otp validation failed.", ex.Message);
                }
            }

            var prevValidationModel = this.fakeDatabase.GetValidationOtp(username);

            // Last validation OTP
            try
            {
                await otpController.Validate(
                    new ValidateRequest
                    {
                        PhoneNumber = username,
                        Otp = "1234",
                        ServiceCode = "Onboarding",
                    });
            }
            catch (Exception ex)
            {
                Assert.Equal("Limit validation tries reach", ex.Message);
            }

            var validationModel = this.fakeDatabase.GetValidationOtp(username);

            // Assert
            Assert.Equal(3, OtpConfiguration.MaxTriesValidateOtp);
            Assert.Equal(0, prevValidationModel.TriesOnboarding);
            Assert.Equal(1, validationModel.TriesOnboarding);
        }

        [Fact]
        public async void ExceedValidateOtpFlowTest()
        {
            // Arrange
            var username = "+521234567890";

            // Act
            var otpController = this.CreateOtpController();
            for (int i = 1; i <= OtpConfiguration.MaxTriesOnboardingFlow * OtpConfiguration.MaxTriesValidateOtp; ++i)
            {
                if (i % OtpConfiguration.MaxTriesValidateOtp == 0)
                {
                    // Last validation OTP
                    try
                    {
                        await otpController.Validate(
                            new ValidateRequest
                            {
                                PhoneNumber = username,
                                Otp = "1234",
                                ServiceCode = "Onboarding",
                            });
                    }
                    catch (Exception ex)
                    {
                        Assert.Equal("Limit validation tries reach", ex.Message);
                    }
                }
                else
                {
                    // Validate OTP
                    try
                    {
                        await otpController.Validate(
                            new ValidateRequest
                            {
                                PhoneNumber = username,
                                Otp = "1234",
                                ServiceCode = "Onboarding",
                            });
                    }
                    catch (Exception ex)
                    {
                        Assert.Equal("The otp validation failed.", ex.Message);
                    }
                }
            }

            // User suspended
            try
            {
                await otpController.Validate(
                new ValidateRequest
                {
                    PhoneNumber = username,
                    Otp = "1234",
                    ServiceCode = "Onboarding",
                });
                Assert.True(false, "Error should be throw because user suspended");
            }
            catch (Exception ex)
            {
                Assert.Equal("User suspended", ex.Message);
            }

            // Assert
            Assert.Equal(3, OtpConfiguration.MaxTriesValidateOtp);
            Assert.Equal(3, OtpConfiguration.MaxTriesOnboardingFlow);
        }

        [Fact]
        public async void ExceedOnboardingAttemptsTest()
        {
            // Arrange
            var username = "+521234567890";

            // Act
            var otpController = this.CreateOtpController();
            var beforeTries = this.fakeDatabase.GetValidationOtp(username);

            // Return to onboarding by validate otp
            for (int i = 1; i < OtpConfiguration.MaxTriesValidateOtp; ++i)
            {
                try
                {
                    await otpController.Validate(
                        new ValidateRequest
                        {
                            PhoneNumber = username,
                            Otp = "1234",
                            ServiceCode = "Onboarding",
                        });
                    Assert.True(false, "Validate should fail");
                }
                catch (Exception ex)
                {
                    Assert.Equal("The otp validation failed.", ex.Message);
                }
            }

            try
            {
                await otpController.Validate(
                    new ValidateRequest
                    {
                        PhoneNumber = username,
                        Otp = "1234",
                        ServiceCode = "Onboarding",
                    });
                Assert.True(false, "Validate should fail");
            }
            catch (Exception ex)
            {
                Assert.Equal("Limit validation tries reach", ex.Message);
            }

            var afterValidate = this.fakeDatabase.GetValidationOtp(username);

            // Return to onboarding by create otps
            for (int i = 0; i < OtpConfiguration.MaxTriesCreateOtp; ++i)
            {
                var createResult = await otpController.Send(
                    new SendOtpRequest
                    {
                        PhoneNumber = username,
                        Channel = "SMS",
                        TemplateId = TemplateIdConstants.SpinPlusOnboarding,
                    });
                var createResponse = Utilities.ProcessOkActionResult<SendOtpRequest>(createResult);
            }

            await Assert.ThrowsAsync<Exception>(
                () => otpController.Send(
                new SendOtpRequest
                {
                    PhoneNumber = username,
                    Channel = "SMS",
                    TemplateId = TemplateIdConstants.SpinPlusOnboarding,
                }));

            var afterCreate = this.fakeDatabase.GetValidationOtp(username);

            // Validate again
            for (int i = 0; i < OtpConfiguration.MaxTriesValidateOtp; ++i)
            {
                await Assert.ThrowsAsync<Exception>(
                    () => otpController.Validate(
                    new ValidateRequest
                    {
                        PhoneNumber = username,
                        Otp = "1234",
                        ServiceCode = "Onboarding",
                    }));
            }

            var afterValidateFinal = this.fakeDatabase.GetValidationOtp(username);

            // User suspended
            try
            {
                await otpController.Validate(
                new ValidateRequest
                {
                    PhoneNumber = username,
                    Otp = "1234",
                    ServiceCode = "Onboarding",
                });
                Assert.True(false, "Error should be throw because user suspended");
            }
            catch (Exception ex)
            {
                Assert.Equal("User suspended", ex.Message);
            }

            try
            {
                await otpController.Send(
                new SendOtpRequest
                {
                    PhoneNumber = username,
                    Channel = "SMS",
                    TemplateId = TemplateIdConstants.SpinPlusOnboarding,
                });
                Assert.True(false, "Error should be throw because user suspended");
            }
            catch (Exception ex)
            {
                Assert.Equal("User suspended", ex.Message);
            }

            // Assert
            Assert.Equal(3, OtpConfiguration.MaxTriesValidateOtp);
            Assert.Equal(3, OtpConfiguration.MaxTriesCreateOtp);
            Assert.Equal(3, OtpConfiguration.MaxTriesOnboardingFlow);
            Assert.Equal(0, beforeTries.TriesOnboarding);
            Assert.Equal(1, afterValidate.TriesOnboarding);
            Assert.Equal(2, afterCreate.TriesOnboarding);
            Assert.Equal(3, afterValidateFinal.TriesOnboarding);
        }

        [Fact]
        public async void ValidateTemplateIdTest()
        {
            // Arrange
            var phoneNumber = "+521234567890";
            var sendPhoneRequest =
                new SendOtpRequest
                {
                    PhoneNumber = phoneNumber,
                    Channel = "SMS",
                    TemplateId = TemplateIdConstants.SpinPlusOnboarding,
                };
            var email = "abc@email.com";
            var sendEmailRequest =
                new SendOtpRequest
                {
                    Email = email,
                    Channel = "EMAIL",
                    TemplateId = TemplateIdConstants.SpinPlusEmailVerification,
                };
            var otpController = this.CreateOtpController();

            // Act
            // Phone number
            var createPhoneResult = await otpController.Send(sendPhoneRequest);
            _ = Utilities.ProcessOkActionResult<SendOtpRequest>(createPhoneResult);
            sendPhoneRequest.TemplateId = "Template-Error";
            await Assert.ThrowsAsync<Exception>(
                () => otpController.Send(sendPhoneRequest));

            // Email
            var createEmailResult = await otpController.Send(sendEmailRequest);
            var requestEmail = Utilities.ProcessOkActionResult<SendOtpRequest>(createEmailResult);
            var template = TemplateIdConstants.SpinPlusEmailVerification;
            Assert.Equal(requestEmail.TemplateId, template);

            // TOdo: actualizar cuando el template se mande desde core
            /* sendEmailRequest.TemplateId = "Template-Error";
             * await Assert.ThrowsAsync<Exception>(
                () => otpController.Send(sendEmailRequest));*/
        }

        [Fact]
        public void PhoneNumberAndEmaiValidateOtpTest()
        {
            // Arrange
            string username = "+521234567890";
            string mail = "usuario.ext@digitalfemsa.com";

            // Act
            try
            {
                IsValidPhoneNumberAndEmailAttribute attribute = new();
                ValidateRequest oValidate = new()
                {
                    PhoneNumber = username,
                    Email = mail,
                    Otp = "1234",
                    ServiceCode = "Onboarding",
                };

                attribute.IsValid(oValidate);
            }
            catch (Exception ex)
            {
                Assert.Equal("You cannot send phone number and email in the same request.", ex.Message);
            }
        }

        [Fact]
        public void NotPhoneNumberAndEmaiValidateOtpTest()
        {
            // Act
            try
            {
                IsValidPhoneNumberAndEmailAttribute attribute = new();
                ValidateRequest oValidate = new()
                {
                    PhoneNumber = string.Empty,
                    Email = string.Empty,
                    Otp = "1234",
                    ServiceCode = "Onboarding",
                };

                attribute.IsValid(oValidate);
            }
            catch (Exception ex)
            {
                Assert.Equal("The phone number or mail must be filled.", ex.Message);
            }
        }

        [Fact]
        public async void OtpWhiteListTest()
        {
            // Arrange
            var phoneNumber = "+521234567890";
            var validateRequest =
                new ValidateRequest
                {
                    PhoneNumber = phoneNumber,
                    Otp = "1234",
                    ServiceCode = "Onboarding",
                };
            OtpConfiguration.OTPMockProves = false;
            var otpController = this.CreateOtpController();

            // Act
            // Validate OTP without whitelist
            try
            {
                await otpController.Validate(validateRequest);
            }
            catch (Exception ex)
            {
                Assert.Equal("The otp validation failed.", ex.Message);
            }

            // Validate OTP with whitelist
            var valueRedis = new OtpMockModel();
            valueRedis.Id = phoneNumber;
            this.fakeDatabase.Saves("mockotp:" + phoneNumber, JsonSerializer.Serialize(valueRedis));
            var validateResult = await otpController.Validate(validateRequest);
            var validateResponse = Utilities.ProcessOkActionResult<ValidOtpResponse>(validateResult);

            // Assert
            Assert.True(!string.IsNullOrWhiteSpace(validateResponse.AccessToken));
        }

        private OTPController CreateOtpController()
        {
            return new OTPController(
                new OtpManager(
                    new RedisManager(
                        this.fakeDatabase.GetRedisCache(),
                        this.loggerRedisManagerMock.Object,
                        new RedisResources()),
                    new AuronixSmsManager(
                        this.httpClientFake,
                        this.loggerValidateRequestMock.Object,
                        new SmsResources()),
                    new WhatsAppManager(
                        this.httpClientFake,
                        this.loggerWhatsAppManagerMock.Object,
                        new WhatsAppResources()),
                    this.loggerRedisManagerMock.Object,
                    new OtpResources(),
                    new EmailManager(
                        this.amazonSimpleEmailServiceMock.Object,
                        new TemplateResources(),
                        new EmailResources(),
                        this.loggerEmailManagerMock.Object),
                    new Serializador()),
                this.loggerOTPControllerMock.Object,
                new OtpControllerResources(),
                new Serializador());
        }
    }
}
