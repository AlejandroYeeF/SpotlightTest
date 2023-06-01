// ---------------------------------------------------------------------------------------------
// <copyright file="TemplateResources.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using SuperbackCiamOTP.Interfaces;

namespace SuperbackCiamOTP.Resources
{
    public class TemplateResources : ITemplateResources
    {
        public string Example_template_2()
        {
            return EmailTemplateResources.spin_plus_email_verification;
        }
    }
}
