// ---------------------------------------------------------------------------------------------
// <copyright file="TypeAuronix.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System;
using System.Text.Json.Serialization;

namespace SuperbackCiamOTP.Entities
{
    public enum TransaccionSmsType
    {
        /// <summary>
        /// Sms request was apparently success
        /// </summary>
        OK = 1,

        /// <summary>
        /// Sms request was wrong
        /// </summary>
        ERROR = 2,
    }

    public enum CodeResponseType
    {
        /// <summary>
        /// Bad credentials
        /// </summary>
        NOT_ALLOWED = -1,

        /// <summary>
        /// Have to retry send the message
        /// </summary>
        RETRY = -5,

        /// <summary>
        /// Error without description
        /// </summary>
        GERENIC_ERROR = -300,

        /// <summary>
        /// Error without description
        /// </summary>
        ANOTHER_ERROR = -99,

        /// <summary>
        /// The message will send to soon
        /// </summary>
        TO_SEND = 1,

        /// <summary>
        /// the message was send
        /// </summary>
        SENDED = 3,

        /// <summary>
        /// Error without description
        /// </summary>
        ERROR = 5,

        /// <summary>
        /// the number is not a mobile number
        /// </summary>
        NO_MOVIL = 6,

        /// <summary>
        /// The number is invalid
        /// </summary>
        INVALID_NUMBER = 10,

        /// <summary>
        /// The number is there in blacklist
        /// </summary>
        BLACK_LIST = 19,

        /// <summary>
        /// The blacklist is not avalible
        /// </summary>
        NOT_AVALIBLE_BLACK_LIST = 19,

        /// <summary>
        ///  Account with balance problems
        /// </summary>
        WITHOUT_BALANCE = 101,
    }
}
