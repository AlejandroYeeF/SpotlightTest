// ---------------------------------------------------------------------------------------------
// <copyright file="WhatsApp.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System;

namespace SuperbackCiamOTP.Entities
{
    public static class WhatsAppConfig
    {
        public static Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public static string ApiKey { get; set; } = string.Empty;

        public static string Uri { get; set; } = string.Empty;

        public static string RequestUri { get; set; } = string.Empty;

        public static string Channel { get; set; } = string.Empty;

        public static string DestinationUserId { get; set; } = string.Empty;

        public static string TemplateId { get; set; } = string.Empty;

        public static string Language { get; set; } = string.Empty;

        public static string Url { get; set; } = string.Empty;

        public static string Mime { get; set; } = string.Empty;

        public static string[] BlackListIds { get; set; } = new string[] { };

        public static List<TransationMetadata> TransationMetadata { get; set; } = new List<TransationMetadata>();

        public static int RetryCount { get; set; } = 0;

        public static int SleepDuration { get; set; } = 0;

        public static int HandledEventsAllowedBeforeBreaking { get; set; } = 0;

        public static int DuritionBreakOf { get; set; } = 0;

        public static int HandlerLifeTime { get; set; } = 0;
    }
}
