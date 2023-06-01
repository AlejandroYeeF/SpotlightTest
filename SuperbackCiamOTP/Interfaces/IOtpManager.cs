// ---------------------------------------------------------------------------------------------
// <copyright file="IOtpManager.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------
using System;
using System.Net;
using StackExchange.Redis;
using SuperbackCiamOTP.Entities;

namespace SuperbackCiamOTP.Interfaces
{
    public interface IOtpManager
    {
        Task<bool> SendOtp(OtpModel otp);

        Task<bool> DeleteOtp(string id);

        Task<bool> CouldGenerateOtp(string identifier);

        Task<bool> CouldValidateOtp(string identifier);

        Task<bool> ValidateOtp(string identifier, ValidateRequest request);

        string GetIdentifier(SendOtpRequest request);

        Task<bool> CheckValidationOtp(string identifier);

        public Task<SettingResponse> ReadAppSettings();
    }
}
