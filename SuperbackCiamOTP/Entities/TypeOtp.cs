// ---------------------------------------------------------------------------------------------
// <copyright file="TypeOtp.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------
using System.Text.Json.Serialization;

namespace SuperbackCiamOTP.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OtpType
    {
        /// <summary>
        /// Numeric.
        /// </summary>
        NUMERIC = 1,
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChannelType
    {
        /// <summary>
        /// Email channel.
        /// </summary>
        EMAIL = 1,

        /// <summary>
        /// Sms channel.
        /// </summary>
        SMS = 2,

        /// <summary>
        /// Whatsapp channel.
        /// </summary>
        WHATSAPP = 3,
    }
}
