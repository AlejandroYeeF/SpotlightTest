// ---------------------------------------------------------------------------------------------
// <copyright file="Request.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SuperbackCiamOTP.Validations;

namespace SuperbackCiamOTP.Entities
{
    public class SendOtpRequest : UserMetaData
    {
        /// <example>SMS</example>
        [Required]
        [EnumDataType(typeof(ChannelType))]
        public string? Channel { get; set; }

        /// <example>Your OTP for verification is</example>
        public string? Message { get; set; }

        /// <example>c1a52602_8687_4104_97bd_71f593ee353c:spin_plus_otp_01</example>
        public string? TemplateId { get; set; }

        /// <example>NUMERIC</example>
        [Required]
        [EnumDataType(typeof(OtpType))]
        [StringLength(15, MinimumLength = 3)]
        public string? Type { get; set; }

        /// <example>254</example>
        [Required]
        public string? ServiceCode { get; set; }
    }

    public class ValidateRequest : UserMetaData
    {
        /// <example>9354</example>
        [Required]
        public string Otp { get; set; } = string.Empty;

        /// <example>onboarding</example>
        [Required]
        public string? ServiceCode { get; set; }
    }

    public class WhatsAppTemplateResquest
    {
        [JsonPropertyName("channel")]
        public string Channel { get; set; } = string.Empty;

        [JsonPropertyName("destinationUserId")]
        public string DestinationUserId { get; set; } = string.Empty;

        [JsonPropertyName("destination")]
        public string Destination { get; set; } = string.Empty;

        [JsonPropertyName("template")]
        public TemplateRequest Template { get; set; } = new TemplateRequest();

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("blackListIds")]
        public string[] BlackListIds { get; set; } = new string[] { };

        [JsonPropertyName("transationMetadata")]
        public List<TransationMetadata> TransationMetadata { get; set; } = new List<TransationMetadata>();
    }

    public class TemplateRequest
    {
        [JsonPropertyName("templateId")]
        public string TemplateId { get; set; } = string.Empty;

        [JsonPropertyName("language")]
        public string Language { get; set; } = string.Empty;

        [JsonPropertyName("vars")]
        public string[] Vars { get; set; } = new string[] { };

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("mime")]
        public string Mime { get; set; } = string.Empty;
    }

    public class TransationMetadata
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;
    }

    public class RequestJwt
    {
        /// <summary>
        /// The username can be a phone number or an email
        /// </summary>
        /// <example>+515555555555</example>
        [Required]
        [StringLength(50, MinimumLength = 6)]
        [IsValidUser]
        public string? Username { get; set; } = string.Empty;

        // <example>Onboarding</example>
        [Required]
        public string? ServiceCode { get; set; } = string.Empty;
    }
}
