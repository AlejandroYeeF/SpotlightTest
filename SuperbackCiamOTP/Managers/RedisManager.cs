// ---------------------------------------------------------------------------------------------
// <copyright file="RedisManager.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------
using StackExchange.Redis;
using SuperbackCiamOTP.Interfaces;

namespace SuperbackCiamOTP.Managers
{
    public class RedisManager : IRedisManager
    {
        private readonly IConnectionMultiplexer redisCache;
        private readonly IDatabase database;
        private readonly ILogger<RedisManager> logger;
        private readonly IRedisResources resources;

        public RedisManager(IConnectionMultiplexer redis, ILogger<RedisManager> logger, IRedisResources resources)
        {
            this.redisCache = redis;
            this.database = this.redisCache.GetDatabase();
            this.logger = logger;
            this.resources = resources;
        }

        public async Task<RedisValue> Save(string value, When when = When.Always)
        {
            try
            {
                return await this.database.StringSetAsync(this.database.KeyRandom(), value, expiry: TimeSpan.FromSeconds(OtpConfiguration.Expiration), when: when);
            }
            catch (Exception e)
            {
                var errorRedis = string.Format(this.resources.AddOtpError(), e.Message);
                this.logger.LogError(errorRedis);
                throw new Exception(errorRedis);
            }
        }

        public async Task<RedisValue> Save(string key, string value, When when = When.Always)
        {
            try
            {
                return await this.database.StringSetAsync(key, value, expiry: TimeSpan.FromSeconds(OtpConfiguration.Expiration), when: when);
            }
            catch (Exception e)
            {
                var errorRedis = string.Format(this.resources.AddOtpError(), e.Message);
                this.logger.LogError(errorRedis);
                throw new Exception(errorRedis);
            }
        }

        public async Task<bool> Delete(string key)
        {
            try
            {
                return await this.database.KeyDeleteAsync(key);
            }
            catch (Exception e)
            {
                var errorRedis = string.Format(this.resources.DeleteOtpError(), e.Message);
                this.logger.LogError(errorRedis);
                throw new Exception(errorRedis);
            }
        }

        public async Task<RedisValue> GetValue(string key)
        {
            try
            {
                return await this.database.StringGetAsync(key);
            }
            catch (Exception e)
            {
                var errorRedis = string.Format(this.resources.GetOtpError(), e.Message);
                this.logger.LogError(errorRedis);
                throw new Exception(errorRedis);
            }
        }

        public async Task<bool> Saves(string key, string value, When when = When.Always)
        {
            try
            {
                await this.database.StringSetAsync(key, value, expiry: TimeSpan.FromSeconds(OtpConfiguration.Expiration), when: when);
                return true;
            }
            catch (Exception e)
            {
                var errorRedis = string.Format(this.resources.AddOtpError(), e.Message);
                this.logger.LogError(errorRedis);
                throw new Exception(errorRedis);
            }
        }
    }
}
