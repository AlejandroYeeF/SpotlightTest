// ---------------------------------------------------------------------------------------------
// <copyright file="PollyDefault.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;
using SuperbackCiamOTP.Validations;

namespace SuperbackCiamOTP.Entities
{
    public class PollyDefault
    {
        public int RetryCount { get; set; } = 0;

        public int SleepDuration { get; set; } = 0;

        public int HandledEventsAllowedBeforeBreaking { get; set; } = 0;

        public int DuritionBreakOf { get; set; } = 0;

        public int HandlerLifeTime { get; set; } = 0;
    }
}
