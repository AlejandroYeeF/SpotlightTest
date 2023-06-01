// ---------------------------------------------------------------------------------------------
// <copyright file="ServiceResources.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using SuperbackCiamOTP.Interfaces;
using SuperbackCiamOTP.Resources;

namespace SuperbackCiamOTP.Managers
{
    public class RedisResources : IRedisResources
    {
        public string AddOtpError()
        {
            return OtpErrorResources.RedisError;
        }

        public virtual string GetOtpError()
        {
            return OtpErrorResources.RedisError;
        }

        public virtual string GetValidationOtpError()
        {
            return OtpErrorResources.RedisError;
        }

        public virtual string DeleteOtpError()
        {
            return OtpErrorResources.RedisError;
        }

        public virtual string AddValidationOtpError()
        {
            return OtpErrorResources.RedisError;
        }

        public virtual string DeleteValidationOtpError()
        {
            return OtpErrorResources.RedisError;
        }
    }

    public class SmsResources : ISmsResources
    {
        public virtual string SingleSendError()
        {
            return OtpErrorResources.SingleSendError;
        }

        public string SmsError()
        {
            return OtpErrorResources.SmsError;
        }
    }

    public class WhatsAppResources : IWhatsAppResources
    {
        public virtual string CreateTemplateError()
        {
            return OtpErrorResources.TemplateError;
        }

        public virtual string ProcessResponseError()
        {
            return OtpErrorResources.ResponseError;
        }
    }

    public class OtpResources : IOtpResources
    {
        public virtual string OtpTypeError()
        {
            return OtpErrorResources.OtpTypeError;
        }

        public virtual string SendOtpError()
        {
            return OtpErrorResources.SendOtpError;
        }

        public virtual string IsSaveRedisStatus()
        {
            return OtpErrorResources.IsSaveRedisStatus;
        }

        public virtual string IsDeleteRedisStatus()
        {
            return OtpErrorResources.IsDeleteRedisStatus;
        }

        public virtual string ChanelTypeInfo()
        {
            return OtpErrorResources.ChannelType;
        }

        public virtual string OtpTypeNotSupported()
        {
            return OtpErrorResources.OtpTypeNotSupported;
        }
    }

    public class MiddlewareResource : IMiddlewareResource
    {
        public string MiddlewareException()
        {
            return OtpErrorResources.MiddlewareException;
        }
    }

    public class OtpControllerResources : IOtpControllerResources
    {
        public virtual string Unauthorized()
        {
            return OtpErrorResources.Unauthorized;
        }
    }

    public class EmailResources : IEmailResources
    {
        public virtual string SimpleEmailException()
        {
            return OtpErrorResources.SimpleEmailException;
        }

        public virtual string TemplateEmailException()
        {
            return OtpErrorResources.TemplateEmailException;
        }
    }
}
