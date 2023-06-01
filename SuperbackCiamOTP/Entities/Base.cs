// ---------------------------------------------------------------------------------------------
// <copyright file="Base.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;
using SuperbackCiamOTP.Validations;

namespace SuperbackCiamOTP.Entities
{
    public static class PayloadConstants
    {
        public const string OnlyNumbers = @"([0-9]+)";
        public const string ValidFormatEmail = @"[a-zA-Z0-9-._]{1,40}@[a-zA-Z0-9_.-]{1,40}\.[a-zA-Z0-9-._]{1,40}$";

        public const string InvalidPhone = "Invalid phone";
        public const string InvalidFormat = "Invalid Format";
    }

    public static class TemplateIdConstants
    {
        public const string SpinPlusOnboarding = "c1a52602_8687_4104_97bd_71f593ee353c:spin_plus_otp_01";
        public const string SpinPlusEmailVerification = "Spin_plus_email_verification";
    }

    public static class TemplateConstants
    {
        public const string SpinPlusOnboarding = "Recuerda no compartir este código. Tu código de verificación Spin Plus es: {0}";
    }

    public class UserMetaData
    {
        /// <example>5215555555555</example>
        [IsValidPhone]
        [StringLength(14, MinimumLength = 10, ErrorMessage = PayloadConstants.InvalidFormat)]
        public string? PhoneNumber { get; set; }

        /// <example>AnyEmail@cool.domain.com</example>
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
    }

    public class AccessResponse
    {
        /// <example>eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZ</example>
        [Required]
        public string AccessToken { get; set; } = string.Empty;
    }
}
