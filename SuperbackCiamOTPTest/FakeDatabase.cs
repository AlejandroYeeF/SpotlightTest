// ---------------------------------------------------------------------------------------------
// <copyright file="FakeDatabase.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System.Text.Json;
using Moq;
using StackExchange.Redis;
using SuperbackCiamOTP.Entities;

namespace SuperbackCiamOTPTest
{
    public class FakeDatabase
    {
        private readonly Dictionary<string, LocalRedisValue> redis;
        private readonly Mock<IConnectionMultiplexer> redisCache;
        private readonly Mock<IDatabase> database;

        public FakeDatabase()
        {
            this.redis = new Dictionary<string, LocalRedisValue>();
            this.redisCache = new Mock<IConnectionMultiplexer>();
            this.database = new Mock<IDatabase>();

            this.InitializeConnectionMultiplexerMock();
            this.InitializeDatabaseMock();
        }

        public void InitializeConnectionMultiplexerMock()
        {
            this.redisCache.Setup(
                x => x.GetDatabase(
                    It.IsAny<int>(),
                    It.IsAny<object>()))
                .Returns(() =>
                {
                    return this.database.Object;
                });
        }

        public void InitializeDatabaseMock()
        {
            this.database.Setup(
                x => x.StringSetAsync(
                    It.IsAny<RedisKey>(),
                    It.IsAny<RedisValue>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<When>()))
                .ReturnsAsync((RedisKey key, RedisValue value, TimeSpan expiry, When _) =>
                {
                    this.redis[key.ToString()] = new LocalRedisValue
                    {
                        Value = value.ToString(),
                        Expiry = expiry.TotalSeconds,
                        CreatedTime = DateTime.Now,
                    };
                    return true;
                });

            this.database.Setup(
                x => x.KeyDeleteAsync(
                    It.IsAny<RedisKey>(),
                    It.IsAny<CommandFlags>()))
                .ReturnsAsync((RedisKey key, CommandFlags _) =>
                {
                    if (!this.redis.ContainsKey(key.ToString()))
                    {
                        return false;
                    }

                    this.redis.Remove(key.ToString());
                    return true;
                });

            this.database.Setup(
                x => x.StringGetAsync(
                    It.IsAny<RedisKey>(),
                    It.IsAny<CommandFlags>()))
                .ReturnsAsync((RedisKey key, CommandFlags _) =>
                {
                    if (!this.redis.ContainsKey(key.ToString()))
                    {
                        return default(RedisValue);
                    }

                    return this.redis[key.ToString()].Value;
                });
        }

        public string? GetValue(string key)
        {
            if (!this.redis.ContainsKey(key))
            {
                return null;
            }

            return this.redis[key].Value;
        }

        public ValidationOtpModel GetValidationOtp(string key)
        {
            var validationDatabase = this.GetValue("validationotp:" + key);
            if (!string.IsNullOrWhiteSpace(validationDatabase))
            {
                var model = JsonSerializer.Deserialize<ValidationOtpModel>(validationDatabase);
                if (model != null)
                {
                    return model;
                }
            }

            return new ValidationOtpModel();
        }

        public OtpModel GetOtpModel(string key)
        {
            var otpDatabase = this.GetValue("otpmodel:" + key);
            if (!string.IsNullOrWhiteSpace(otpDatabase))
            {
                var model = JsonSerializer.Deserialize<OtpModel>(otpDatabase);
                if (model != null)
                {
                    return model;
                }
            }

            return new OtpModel();
        }

        public void Saves(string key, string value)
        {
            this.redis[key] = new LocalRedisValue
            {
                Value = value,
            };
        }

        public IConnectionMultiplexer GetRedisCache()
        {
            return this.redisCache.Object;
        }

        public struct LocalRedisValue
        {
            public string Value;
            public double Expiry;
            public DateTime CreatedTime;
        }
    }
}
