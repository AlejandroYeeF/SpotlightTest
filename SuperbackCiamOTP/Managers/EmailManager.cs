﻿﻿// ---------------------------------------------------------------------------------------------
// <copyright file="EmailManager.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System.Reflection;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using SuperbackCiamOTP.Entities;
using SuperbackCiamOTP.Interfaces;

namespace SuperbackCiamOTP.Managers
{
    public class EmailManager : IEmailManager
    {
        private readonly IAmazonSimpleEmailService amazonSimpleEmailService;
        private readonly ITemplateResources templateResources;
        private readonly IEmailResources resources;
        private readonly ILogger logger;

        public EmailManager(
            IAmazonSimpleEmailService amazonSimpleEmailService,
            ITemplateResources templateResources,
            IEmailResources resources,
            ILogger<EmailManager> logger)
        {
            this.amazonSimpleEmailService = amazonSimpleEmailService;
            this.templateResources = templateResources;
            this.resources = resources;
            this.logger = logger;
        }

        public async Task<bool> SendSimpleEmail(string toAddresses, string message)
        {
            try
            {
                SendEmailRequest sendEmailRequest = new SendEmailRequest()
                {
                    Destination = new Destination() { ToAddresses = new List<string>() { toAddresses } },
                    Message = new Message()
                    {
                        Body = new Body()
                        {
                            Html = new Content()
                            {
                                Charset = "UTF-8",
                                Data = message,
                            },
                        },
                        Subject = new Content()
                        {
                            Charset = "UTF-8",
                            Data = EmailConfig.Subject,
                        },
                    },
                    Source = EmailConfig.From,
                };

                var sendResult = await this.amazonSimpleEmailService.SendEmailAsync(sendEmailRequest);

                if (sendResult.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                var error = string.Format(this.resources.SimpleEmailException(), e.Message);
                this.logger.LogError(error);
                throw new Exception(error);
            }
        }

        public async Task<bool> SendEmailTemplate(string toAddresses, string otp, string template)
        {
            try
            {
                var templateDataObject = this.GetTemplateData(template, otp);

                var sendResult = await this.amazonSimpleEmailService.SendTemplatedEmailAsync(new SendTemplatedEmailRequest
                {
                    Template = EmailConfig.TemplateName,
                    Destination = new Destination
                    {
                        ToAddresses = new List<string> { toAddresses },
                    },

                    TemplateData = templateDataObject,
                    Source = EmailConfig.Source,
                });

                if (sendResult.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                var error = string.Format(this.resources.TemplateEmailException(), e.Message);
                this.logger.LogError(error);
                throw new Exception(error);
            }
        }

        private string GetTemplateData(string template, string otp)
        {
            TemplateType channelType;
            _ = Enum.TryParse<TemplateType>(template, out channelType);
            string templateData = string.Empty;
            switch (channelType)
            {
                case TemplateType.OTP_TEMPLATE:
                case TemplateType.Spin_plus_email_verification:
                    EmailConfig.TemplateName = TemplateType.Spin_plus_email_verification.ToString();
                    templateData = "{" + string.Format(this.templateResources.Example_template_2(), otp) + "}";
                    break;
            }

            return templateData;
        }
    }
}
