﻿// ---------------------------------------------------------------------------------------------
// <copyright file="Email.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System;
using System.Text.Json.Serialization;

namespace SuperbackCiamOTP.Entities
{
    public static class EmailConfig
    {
        public static string From { get; set; } = string.Empty;

        public static string Subject { get; set; } = string.Empty;

        public static string AccessKeyId { get; set; } = string.Empty;

        public static string SecretKeyId { get; set; } = string.Empty;

        public static string TemplateName { get; set; } = string.Empty;

        public static string SourceArn { get; set; } = string.Empty;

        public static string Source { get; set; } = string.Empty;

        public static string RegionEndpoint { get; set; } = string.Empty;
    }
}
