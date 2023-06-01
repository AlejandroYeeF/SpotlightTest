// ---------------------------------------------------------------------------------------------
// <copyright file="StaticLoggerFactory.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System.Collections.Concurrent;

namespace SuperbackCiamOTP.Managers
{
    public static class StaticLoggerFactory
    {
        private static ILoggerFactory? loggerFactory;

        private static ConcurrentDictionary<Type, ILogger> loggerByType = new();

        public static void Initialize(ILoggerFactory loggerFactory)
        {
            if (StaticLoggerFactory.loggerFactory is not null)
            {
                throw new InvalidOperationException("StaticLogger already initialized!");
            }

            StaticLoggerFactory.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public static ILogger GetStaticLogger<T>()
        {
            if (loggerFactory is null)
            {
                throw new InvalidOperationException("StaticLogger is not initialized yet.");
            }

            return loggerByType
                .GetOrAdd(typeof(T), loggerFactory.CreateLogger<T>());
        }
    }
}
