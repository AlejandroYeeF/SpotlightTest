// ---------------------------------------------------------------------------------------------
// <copyright file="ErrorCodes.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http.HttpResults;
using SuperbackCIAM.Middleware;
using SuperbackCiamOTP.Validations;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SuperbackCIAM.Middleware
{
    public static class ErrorCodes
    {
        public static List<Error> ErrorList { get; set; } = new List<Error>();

        public static void AddError(List<string> expectedError, HttpStatusCode httpsStatusCode, string code, string type, string message)
        {
            ErrorList.Add(new Error
            {
                ExpectedError = expectedError,
                StatusCodes = httpsStatusCode,
                Code = code,
                Types = type,
                Message = message,
            });
        }

        public static Error DefaultError()
        {
            return new Error()
            {
                ExpectedError = new List<string>(),
                StatusCodes = HttpStatusCode.InternalServerError,
                Code = "SPOP-500",
                Types = "OTP",
                Message = "System failure",
            };
        }

        public static void Start(IConfiguration configuration)
        {
            ErrorList = new List<Error>();
            AddError(
                new List<string>()
                {
                    GenericMessages.PhoneInvalid,
                    GenericMessages.UserInvalid,
                    "The field Channel is invalid.",
                    "Data does not pass validations",
                    "Invalid phone.",
                    "Invalid username.",
                },
                HttpStatusCode.BadRequest,
                "SPOP-400",
                "OTP",
                "Data does not pass validations.");
            AddError(
                new List<string>()
                {
                    "The phone number is out of coverage.",
                },
                HttpStatusCode.BadRequest,
                "SPOP-400-1",
                "OTP",
                "The phone number is out of coverage.");
            AddError(
                new List<string>()
                {
                    "Channel type not supported yet.",
                    "Otp type not supported yet.",
                },
                HttpStatusCode.ServiceUnavailable,
                "SPOP-503",
                "OTP",
                "The service is not available.");
            AddError(
                new List<string>()
                {
                    "Limit send tries reach",
                    "Reach maximum onboarding tries",
                },
                HttpStatusCode.Forbidden,
                "SPOP-403-1",
                "OTP",
                "Send back to the start, OTP tries send exceeded.");
            AddError(
                new List<string>()
                {
                    "Limit validation tries reach",
                },
                HttpStatusCode.Forbidden,
                "SPOP-403-2",
                "OTP",
                "Send back the start, OTP tries exceeded.");
            AddError(
                new List<string>()
                {
                    "Temporal suspension for spam retries",
                },
                HttpStatusCode.TooManyRequests,
                "SPOP-429",
                "OTP",
                "Suspended by tries exceeded.");
            AddError(
                new List<string>()
                {
                    "The otp validation failed.",
                },
                HttpStatusCode.Unauthorized,
                "SPOP-401-1",
                "OTP",
                "OTP Invalid or expired.");
            AddError(
                new List<string>()
                {
                    "User suspended",
                },
                HttpStatusCode.Forbidden,
                "SPOP-403-3",
                "OTP",
                "Suspended user by exceed requests.");
            AddError(
                new List<string>()
                {
                    "SMS Error",
                    "Delete Error",
                    "RedisError",
                    "CreateTemplateError",
                    "Error response",
                },
                HttpStatusCode.InternalServerError,
                "SPOP-500",
                "OTP",
                "System Failure.");
            AddError(
                new List<string>()
                {
                  GenericMessages.PhoneNumberAndEmail,
                },
                HttpStatusCode.BadRequest,
                "SPOP-400-2",
                "OTP",
                GenericMessages.PhoneNumberAndEmail);
            AddError(
                new List<string>()
                {
                   GenericMessages.NotPhoneNumberAndEmail,
                },
                HttpStatusCode.BadRequest,
                "SPOP-400-3",
                "OTP",
                GenericMessages.NotPhoneNumberAndEmail);
        }

        public static Error GetError(string message)
        {
            var errorCode = DefaultError();
            foreach (var item in ErrorList)
            {
                errorCode = item.ExpectedError.Any(x => message.ToUpper().Contains(x.ToUpper())) ? item : errorCode;
            }

            return errorCode;
        }
    }

    public class Error
    {
        public HttpStatusCode StatusCodes { get; set; } = 0;

        public string Code { get; set; } = string.Empty;

        public string Types { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public List<string> ExpectedError { get; set; } = new List<string>();
    }
}
