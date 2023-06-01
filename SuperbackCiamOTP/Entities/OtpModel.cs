// ---------------------------------------------------------------------------------------------
// <copyright file="OtpModel.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------
using System;
using Redis.OM.Modeling;

namespace SuperbackCiamOTP.Entities
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "Otp" })]
    public class OtpModel
    {
        [RedisIdField]
        public string Id { get; set; } = string.Empty;

        [Searchable]
        public string Type { get; set; } = string.Empty;

        [Searchable]
        public string Channel { get; set; } = string.Empty;

        [Searchable]
        public string CodeService { get; set; } = string.Empty;

        [Searchable]
        public string Otp { get; set; } = string.Empty;

        [Searchable]
        public string PhoneNumber { get; set; } = string.Empty;

        [Searchable]
        public string Email { get; set; } = string.Empty;

        [Searchable]
        public string Message { get; set; } = string.Empty;

        [Searchable]
        public string TemplateId { get; set; } = string.Empty;

        [Searchable]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Searchable]
        public bool IsUsed { get; set; } = false;

        [Searchable]
        public int Expiration { get; set; }
    }

    [Document(StorageType = StorageType.Json, Prefixes = new[] { "ValidationOtp" })]
    public class ValidationOtpModel
    {
        [RedisIdField]
        public string Id { get; set; } = string.Empty;

        [Searchable]
        public DateTime DateTriesCreate { get; set; } = DateTime.UtcNow.AddMinutes(-1.0);

        [Searchable]
        public DateTime DateValidateOtp { get; set; } = DateTime.UtcNow.AddMinutes(-1.0);

        [Searchable]
        public DateTime DateTriesOnboarding { get; set; } = DateTime.UtcNow.AddMinutes(-1.0);

        [Searchable]
        public DateTime DateBlocked { get; set; } = DateTime.UtcNow.AddDays(-1.0);

        [Searchable]
        public bool IsBlocked { get; set; } = false;

        [Searchable]
        public int TriesCreateOtp { get; set; } = 0;

        [Searchable]
        public int TriesValidateOtp { get; set; } = 0;

        [Searchable]
        public int TriesOnboarding { get; set; } = 0;
    }

    [Document(StorageType = StorageType.Json, Prefixes = new[] { "MockOtp" })]
    public class OtpMockModel
    {
        [RedisIdField]
        public string Id { get; set; } = string.Empty;
    }
}
