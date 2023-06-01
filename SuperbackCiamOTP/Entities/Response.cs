// ---------------------------------------------------------------------------------------------
// <copyright file="Response.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SuperbackCiamOTP.Entities
{
    public class OtpResponse : SendOtpRequest
    {
    }

    public class ValidOtpResponse : AccessResponse
    {
    }

    public class SmsResponsee
    {
        [JsonPropertyName("transaccionEjecutada")]
        public string TransaccionEjecutada { get; set; } = string.Empty;

        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;

        [JsonPropertyName("errorCode")]
        public int ErrorCode { get; set; } = 0;

        [JsonPropertyName("resultadoTransaccion")]
        public int ResultadoTransaccion { get; set; } = 0;

        public string DetilResult { get; set; } = string.Empty;
    }

    public class WhatsAppResponse
    {
        public bool IsCreated { get; set; }

        public string Message { get; set; } = string.Empty;

        public string Error { get; set; } = string.Empty;
    }

    public class FailedWhatsAppResponse
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("errors")]
        public List<ErrorsWhatsApp> Errors { get; set; } = new List<ErrorsWhatsApp>();

        [JsonPropertyName("correlationId")]
        public string CorrelationId { get; set; } = string.Empty;
    }

    public class ErrorsWhatsApp
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("detail")]
        public string Detail { get; set; } = string.Empty;
    }

    public class JwtResponse
    {
        /// <example>v1.McGUUd_CL7lc4UjVa2jlAgAG7lCyif14ItPqa_VyqXbeLfR1IpHdqu02QT3XuSsGekr0zfh3PJBszwsWGnC8lJw</example>
        [Required]
        public string Jwt { get; set; } = string.Empty;
    }

    public class SettingResponse
    {
        // <example>true</example>
        [Required]
        public Dictionary<string, List<string>> Setting1 { get; set; } = new Dictionary<string, List<string>>();

        public string Setting2 { get; set; } = string.Empty;

        public string Setting3 { get; set; } = string.Empty;
    }
}
