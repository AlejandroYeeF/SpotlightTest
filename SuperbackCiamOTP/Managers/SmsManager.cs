// ---------------------------------------------------------------------------------------------
// <copyright file="SmsManager.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System.Web;
using SuperbackCiamOTP.Entities;
using SuperbackCiamOTP.Interfaces;

namespace SuperbackCiamOTP.Managers
{
    public class AuronixSmsManager : ISmsManager
    {
        private const string ProviderName = "Auronix";
        private readonly Dictionary<string, string> headers = SmsConfig.Headers;
        private readonly string requestUri = SmsConfig.RequestUri;
        private readonly ILogger logger;
        private readonly ISmsResources resources;
        private HttpClient httpClient;

        public AuronixSmsManager(HttpClient httpClient, ILogger<ValidateRequest> logger, ISmsResources resources)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            this.resources = resources;
        }

        public async Task<string> SingleSend(string phoneNumber, string message)
        {
            try
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{this.requestUri}?{this.CreateQueryString(phoneNumber, message)}");
                var response = await this.httpClient.SendAsync(httpRequest);
                this.logger.LogInformation($"Auronix SMS Service, Respose Satus Code:{response.StatusCode}");
                return await this.StatusResponseAsync(response);
            }
            catch (Exception e)
            {
                var errorSms = string.Format(this.resources.SingleSendError(), ProviderName, e.Message);
                this.logger.LogError(errorSms);
                return string.Format(this.resources.SmsError(), e.Message);
            }
        }

        private static void GetEnums(SmsResponsee responsee, out TransaccionSmsType transaccionType, out CodeResponseType responseType)
        {
            _ = Enum.TryParse<TransaccionSmsType>(responsee.TransaccionEjecutada, out transaccionType);
            _ = Enum.TryParse<CodeResponseType>(responsee.DetilResult, out responseType);
        }

        private async Task<string> StatusResponseAsync(HttpResponseMessage response)
        {
            TransaccionSmsType transaccionType;
            CodeResponseType responseType;
            var result = await response.Content.ReadFromJsonAsync<SmsResponsee>() ?? new SmsResponsee();
            result.DetilResult = result.ErrorCode == 0 ? result.ResultadoTransaccion.ToString() : result.ErrorCode.ToString();
            GetEnums(result, out transaccionType, out responseType);
            this.logger.LogInformation($"Detail response SMS: {result.DetilResult} \nType transaction: {transaccionType.ToString()}\n Response Type: {responseType.ToString()}");
            return $"{transaccionType.ToString()}:{responseType}";
        }

        private string CreateQueryString(string phoneNumber, string message)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["cte"] = SmsConfig.Cte;
            query["encpwd"] = SmsConfig.Encpwd;
            query["email"] = SmsConfig.Email;
            query["mtipo"] = "SMS";
            query["numtel"] = phoneNumber;
            query["msg"] = message;
            query["json"] = "1";
            return query.ToString() ?? string.Empty;
        }
    }
}
