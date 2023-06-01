// ---------------------------------------------------------------------------------------------
// <copyright file="ErrorDetails.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SuperbackCIAM.Middleware
{
    internal class ErrorDetails
    {
        public ErrorDetails(string? errorCode, string? message, string? detail)
        {
            this.ErrorCode = errorCode;
            this.Message = message;
            this.Detail = detail;
        }

        [JsonPropertyName("errorCode")]
        public string? ErrorCode { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("detail")]
        public string? Detail { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
