// ---------------------------------------------------------------------------------------------
// <copyright file="ExceptionExtensions.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System;
using SuperbackCIAM.Middleware;

namespace SuperbackCIAM.Middleware
{
    public static class ExceptionExtensions
    {
        public static void ConfigureCustomException(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionManager>();
        }
    }
}
