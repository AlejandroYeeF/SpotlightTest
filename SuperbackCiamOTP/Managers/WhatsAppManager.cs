// ---------------------------------------------------------------------------------------------
// <copyright file="WhatsAppManager.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System.Net;
using System.Text.Json;
using SuperbackCiamOTP.Entities;
using SuperbackCiamOTP.Interfaces;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace SuperbackCiamOTP.Managers
{
    public sealed class WhatsAppManager : IWhatsAppManager
    {
        private const string ProviderName = "Auronix";
        private const string BadRequest = "Payload invalid or invalid values.";
        private const string Created = "Success";
        private const string Unauthorized = "Invalid Token API";
        private const string InternalServerError = "Internal server error on provider.";
        private readonly string requestUri = WhatsAppConfig.RequestUri;
        private readonly Dictionary<string, string> headers = WhatsAppConfig.Headers;
        private readonly ILogger logger;
        private readonly IWhatsAppResources resources;
        private HttpClient httpClient;

        public WhatsAppManager(HttpClient httpClient, ILogger<WhatsAppManager> logger, IWhatsAppResources resources)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            this.resources = resources;
        }

        public async Task<WhatsAppResponse> CreateTemplate(string phoneNumber, string templateId, string otp)
        {
            phoneNumber = phoneNumber.Replace("+", string.Empty);
            WhatsAppResponse whatsAppResponse = new();
            this.SetHeaders(this.headers);
            var template = this.GetTemplate(phoneNumber, templateId, otp);
            try
            {
                var json = JsonSerializer.Serialize(template);
                var response = await this.httpClient.PostAsJsonAsync(this.requestUri, template);
                this.logger.LogInformation($"Auronix WhatsApp Service, Respose Satus Code:{response.StatusCode}");
                await ProcessResponse(whatsAppResponse, response, this.logger, this.resources);
                this.logger.LogInformation($"Is send Otp?:{whatsAppResponse.IsCreated}");
            }
            catch (Exception e)
            {
                var errorWhatsApp = string.Format(this.resources.CreateTemplateError(), ProviderName, e.Message);
                this.logger.LogError(errorWhatsApp);
                throw new Exception(errorWhatsApp);
            }

            return whatsAppResponse;
        }

        private static async Task ProcessResponse(WhatsAppResponse whatsAppResponse, HttpResponseMessage response, ILogger logger, IWhatsAppResources resources)
        {
            string errorWhatsApp;
            switch (response.StatusCode)
            {
                case HttpStatusCode.Created:
                    whatsAppResponse.IsCreated = true;
                    whatsAppResponse.Message = Created;
                    errorWhatsApp = string.Format(resources.CreateTemplateError(), ProviderName, JsonSerializer.Serialize(whatsAppResponse));
                    logger.LogInformation(errorWhatsApp);
                    break;
                case HttpStatusCode.Unauthorized:
                    whatsAppResponse.Error = await response.Content.ReadAsStringAsync();
                    whatsAppResponse.Message = Unauthorized;
                    errorWhatsApp = string.Format(resources.CreateTemplateError(), ProviderName, JsonSerializer.Serialize(whatsAppResponse));
                    logger.LogError(JsonSerializer.Serialize(whatsAppResponse));
                    break;
                case HttpStatusCode.BadRequest:
                    var badrequest = await response.Content.ReadFromJsonAsync<FailedWhatsAppResponse>() ?? new FailedWhatsAppResponse();
                    whatsAppResponse.Error = badrequest.Description;
                    whatsAppResponse.Message = BadRequest;
                    errorWhatsApp = string.Format(resources.CreateTemplateError(), ProviderName, JsonSerializer.Serialize(whatsAppResponse));
                    logger.LogError(JsonSerializer.Serialize(whatsAppResponse));
                    break;
                case HttpStatusCode.InternalServerError:
                    var internalServerError = await response.Content.ReadFromJsonAsync<FailedWhatsAppResponse>() ?? new FailedWhatsAppResponse();
                    whatsAppResponse.Error = internalServerError.Description;
                    whatsAppResponse.Message = InternalServerError;
                    errorWhatsApp = string.Format(resources.CreateTemplateError(), ProviderName, JsonSerializer.Serialize(whatsAppResponse));
                    logger.LogError(JsonSerializer.Serialize(whatsAppResponse));
                    break;
                default:
                    errorWhatsApp = string.Format(resources.CreateTemplateError(), ProviderName, $"Provider: status code unknown");
                    logger.LogError(JsonSerializer.Serialize(whatsAppResponse));
                    throw new Exception("Error response");
            }
        }

        private WhatsAppTemplateResquest GetTemplate(string phoneNumber, string templateId, string otp)
        {
            templateId = string.IsNullOrEmpty(templateId) ? WhatsAppConfig.TemplateId : templateId;
            return new WhatsAppTemplateResquest
            {
                Channel = WhatsAppConfig.Channel,
                DestinationUserId = WhatsAppConfig.DestinationUserId,
                Destination = phoneNumber,
                Template = new TemplateRequest
                {
                    TemplateId = templateId,
                    Language = WhatsAppConfig.Language,
                    Vars = new string[] { otp },
                    Url = WhatsAppConfig.Url,
                    Mime = WhatsAppConfig.Mime,
                },
                BlackListIds = WhatsAppConfig.BlackListIds,
                TransationMetadata = WhatsAppConfig.TransationMetadata,
            };
        }

        private void SetHeaders(Dictionary<string, string> headers)
        {
            this.httpClient.DefaultRequestHeaders.Clear();
            foreach (var header in headers)
            {
                this.httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }
    }
}
