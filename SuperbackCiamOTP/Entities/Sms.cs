// ---------------------------------------------------------------------------------------------
// <copyright file="Sms.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System;

namespace SuperbackCiamOTP.Entities
{
    public static class SmsConfig
    {
        public static Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public static string Uri { get; set; } = string.Empty;

        public static string RequestUri { get; set; } = string.Empty;

        public static string Cte { get; set; } = string.Empty;

        public static string Encpwd { get; set; } = string.Empty;

        public static string Email { get; set; } = string.Empty;

        public static PollyDefault PollyDefault { get; set; } = new PollyDefault();
    }
}
