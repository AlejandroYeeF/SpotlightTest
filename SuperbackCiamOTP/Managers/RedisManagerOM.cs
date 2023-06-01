// ---------------------------------------------------------------------------------------------
// <copyright file="RedisManagerOM.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------
using System;
using Microsoft.Extensions.Logging;
using Redis.OM;
using Redis.OM.Searching;
using Redis.OM.Searching.Query;
using SuperbackCiamOTP.Entities;
using SuperbackCiamOTP.Interfaces;

namespace SuperbackCiamOTP.Managers
{
    public class RedisManagerOM : IRedisManagerOM
    {
        private readonly RedisConnectionProvider provider;
        private readonly RedisCollection<ValidationOtpModel> validationOtpModel;
        private readonly RedisCollection<OtpModel> otpModel;
        private readonly ILogger logger;
        private readonly IRedisResources resources;

        public RedisManagerOM(RedisConnectionProvider provider, ILogger<RedisManagerOM> logger, IRedisResources resources)
        {
            this.provider = provider;
            this.logger = logger;
            this.otpModel = (RedisCollection<OtpModel>)provider.RedisCollection<OtpModel>();
            this.validationOtpModel = (RedisCollection<ValidationOtpModel>)provider.RedisCollection<ValidationOtpModel>();
            this.resources = resources;
        }

        public async Task<bool> AddOtp(OtpModel otp, WhenKey whenKey = WhenKey.Always)
        {
            try
            {
                await this.otpModel.InsertAsync(otp, when: whenKey, TimeSpan.FromSeconds(OtpConfiguration.Expiration));
                return true;
            }
            catch (Exception e)
            {
                var errorRedis = string.Format(this.resources.AddOtpError(), e.Message);
                this.logger.LogError(errorRedis);
                throw new Exception(errorRedis);
            }
        }

        public async Task<OtpModel?> GetOtp(string identifier)
        {
            try
            {
                var otp = await this.otpModel.FindByIdAsync(identifier) ?? new OtpModel();
                this.logger.LogInformation($"looking for : {identifier} ; finded:{otp.Id}:{otp.Otp}");
                return otp;
            }
            catch (Exception e)
            {
                var errorRedis = string.Format(this.resources.GetOtpError(), e.Message);
                this.logger.LogError(errorRedis);
                throw new Exception(errorRedis);
            }
        }

        public async Task<bool> DeleteOtp(string identifier)
        {
            try
            {
                var otp = await this.otpModel.FindByIdAsync(identifier) ?? new OtpModel();
                await this.otpModel.DeleteAsync(otp);
                this.logger.LogInformation($"delete for : {identifier} ; finded:{otp.Id}:{otp.Otp}");
                return true;
            }
            catch (Exception e)
            {
                var errorRedis = string.Format(this.resources.DeleteOtpError(), e.Message);
                this.logger.LogError(errorRedis);
                throw new Exception(errorRedis);
            }
        }

        public async Task<bool> AddValidationOtp(ValidationOtpModel validationOtp, WhenKey whenKey = WhenKey.Always)
        {
            try
            {
                if (await this.validationOtpModel.InsertAsync(validationOtp, when: whenKey, TimeSpan.FromSeconds(OtpConfiguration.Expiration)) != null)
                {
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                var errorRedis = string.Format(this.resources.AddValidationOtpError(), e.Message);
                this.logger.LogError(errorRedis);
                throw new Exception(errorRedis);
            }
        }

        public async Task<ValidationOtpModel?> GetValidationOtp(string identifier)
        {
            try
            {
                return await this.validationOtpModel.FindByIdAsync(identifier);
            }
            catch (Exception e)
            {
                var errorRedis = string.Format(this.resources.GetValidationOtpError(), e.Message);
                this.logger.LogError(errorRedis);
                throw new Exception(errorRedis);
            }
        }

        public async Task<bool> DeleteValidationOtp(string identifier)
        {
            try
            {
                var validationOtp = await this.validationOtpModel.FindByIdAsync(identifier);
                if (validationOtp != null)
                {
                    await this.validationOtpModel.DeleteAsync(validationOtp);
                }

                return true;
            }
            catch (Exception e)
            {
                var errorRedis = string.Format(this.resources.DeleteValidationOtpError(), e.Message);
                this.logger.LogError(errorRedis);
                throw new Exception(errorRedis);
            }
        }
    }
}
