// ---------------------------------------------------------------------------------------------
// <copyright file="IRedisManager.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------
using StackExchange.Redis;

namespace SuperbackCiamOTP.Interfaces
{
    public interface IRedisManager
    {
        Task<RedisValue> Save(string value, When when = When.Always);
        Task<RedisValue> Save(string key, string value, When when = When.Always);

        Task<bool> Delete(string key);

        Task<RedisValue> GetValue(string key);

        Task<bool> Saves(string key, string value, When when = When.Always);
    }
}
