// ---------------------------------------------------------------------------------------------
// <copyright file="IWhatsAppManager.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using SuperbackCiamOTP.Entities;

namespace SuperbackCiamOTP.Interfaces
{
    public interface IWhatsAppManager
    {
        Task<WhatsAppResponse> CreateTemplate(string phoneNumber, string message, string otp);
    }
}
