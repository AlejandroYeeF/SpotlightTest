// ---------------------------------------------------------------------------------------------
// <copyright file="IEmailManager.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

namespace SuperbackCiamOTP.Managers
{
    public interface IEmailManager
    {
        Task<bool> SendSimpleEmail(string toAddresses, string message);

        Task<bool> SendEmailTemplate(string toAddresses, string otp, string template);
    }
}
