// ---------------------------------------------------------------------------------------------
// <copyright file="IndexCreationService.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------
using System;
using Redis.OM;
using SuperbackCiamOTP.Entities;

namespace SuperbackCiamOTP.Managers
{
    /// <summary>
    /// The most correct way to manage this is to spin the index creation out into a Hosted Service,
    /// which will run when the app spins up. Create a HostedServices directory and add IndexCreationService.cs to that.
    /// </summary>
    public class IndexCreationService : IHostedService
    {
        private readonly RedisConnectionProvider provider;

        public IndexCreationService(RedisConnectionProvider provider)
        {
            this.provider = provider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var info = this.provider.Connection.Execute("FT._LIST").ToArray().Select(x => x.ToString());
            if (info.All(x => x != "otpmodel-idx") && info.All(x => x != "validationotp-idx"))
            {
                await this.provider.Connection.CreateIndexAsync(typeof(OtpModel));
                await this.provider.Connection.CreateIndexAsync(typeof(ValidationOtpModel));
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
