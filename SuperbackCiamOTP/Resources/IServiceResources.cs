// ---------------------------------------------------------------------------------------------
// <copyright file="IServiceResources.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

namespace SuperbackCiamOTP.Interfaces
{
    public interface IRedisResources
    {
        string GetOtpError();

        string AddOtpError();

        string DeleteOtpError();

        string AddValidationOtpError();

        string DeleteValidationOtpError();

        string GetValidationOtpError();
    }

    public interface ISmsResources
    {
        string SingleSendError();

        string SmsError();
    }

    public interface IWhatsAppResources
    {
        string CreateTemplateError();

        string ProcessResponseError();
    }

    public interface IOtpResources
    {
        string SendOtpError();

        string OtpTypeError();

        string IsSaveRedisStatus();

        string IsDeleteRedisStatus();

        string ChanelTypeInfo();

        string OtpTypeNotSupported();
    }

    public interface IMiddlewareResource
    {
        string MiddlewareException();
    }

    public interface IOtpControllerResources
    {
        string Unauthorized();
    }

    public interface IEmailResources
    {
        string SimpleEmailException();

        string TemplateEmailException();
    }

    public interface ITemplateResources
    {
        string Example_template_2();
    }
}
