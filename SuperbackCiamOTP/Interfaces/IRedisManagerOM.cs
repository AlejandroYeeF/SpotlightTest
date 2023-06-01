// ---------------------------------------------------------------------------------------------
// <copyright file="IRedisManagerOM.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------
using System;
using Redis.OM;
using SuperbackCiamOTP.Entities;

namespace SuperbackCiamOTP.Interfaces
{
    public interface IRedisManagerOM
    {
        Task<bool> AddOtp(OtpModel otp, WhenKey whenKey = WhenKey.Always);

        Task<OtpModel?> GetOtp(string identifier);

        Task<bool> DeleteOtp(string identifier);

        Task<bool> AddValidationOtp(ValidationOtpModel validationOtp, WhenKey whenKey = WhenKey.Always);

        Task<ValidationOtpModel?> GetValidationOtp(string identifier);

        Task<bool> DeleteValidationOtp(string identifier);
    }
}
