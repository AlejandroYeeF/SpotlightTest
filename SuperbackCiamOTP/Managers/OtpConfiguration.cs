// ---------------------------------------------------------------------------------------------
// <copyright file="OtpConfiguration.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using Amazon.SimpleEmail;
using Polly;
using Polly.Extensions.Http;
using Redis.OM;
using StackExchange.Redis;
using SuperbackCiamOTP.Entities;
using SuperbackCiamOTP.Interfaces;
using SuperbackCiamOTP.Resources;

namespace SuperbackCiamOTP.Managers
{
    public class OtpConfiguration
    {
        public static string HostName { get; set; } = string.Empty;

        public static string PortNumber { get; set; } = string.Empty;

        public static string Password { get; set; } = string.Empty;

        public static string RedisConnection { get; set; } = string.Empty;

        public static string RedisConnectionLocal { get; set; } = string.Empty;

        public static int Expiration { get; set; } = 86400;

        public static int MinLength { get; set; } = 1000;

        public static int MaxLength { get; set; } = 10000;

        public static int SecondBetweenRequest { get; set; } = 60;

        public static int MaxTriesCreateOtp { get; set; } = 3;

        public static int MaxTriesValidateOtp { get; set; } = 3;

        public static int MaxTriesOnboardingFlow { get; set; } = 3;

        public static bool OTPMockProves { get; set; } = false;

        public static string Domain { get; set; } = string.Empty;

        public static string Audience { get; set; } = string.Empty;

        public static Dictionary<string, List<string>> CountriesAllowed { get; set; } = new Dictionary<string, List<string>>();

        public static string Issuer { get; set; } = string.Empty;

        public static string SecretKey { get; set; } = string.Empty;

        public static string ExpiresToken { get; set; } = string.Empty;

        public static Dictionary<string, List<string>> ReadApp1 { get; set; } = new Dictionary<string, List<string>>();

        public static string ReadApp2 { get; set; } = string.Empty;

        public static string ReadApp3 { get; set; } = string.Empty;

        public static void GetInitialConfiguration(IConfiguration configuration)
        {
            HostName = configuration.GetValue<string>("Redis:HostName") ?? string.Empty;
            PortNumber = configuration.GetValue<string>("Redis:PortNumber") ?? string.Empty;
            Password = configuration.GetValue<string>("Redis:Password") ?? string.Empty;
            Expiration = configuration.GetValue<int?>("Otp:Expire") ?? 86400;
            MaxLength = configuration.GetValue<int>("Otp:MaxLength");
            MinLength = configuration.GetValue<int>("Otp:MinLength");
            OTPMockProves = configuration.GetValue<bool>("OtpManagerConfig:OtpMockProves");
            SecondBetweenRequest = configuration.GetValue<int>("OtpManagerConfig:SecondBetweenRequest");
            MaxTriesCreateOtp = configuration.GetValue<int>("OtpManagerConfig:MaxTriesCreateOtp");
            MaxTriesValidateOtp = configuration.GetValue<int>("OtpManagerConfig:MaxTriesValidateOtp");
            MaxTriesOnboardingFlow = configuration.GetValue<int>("OtpManagerConfig:MaxTriesOnboardingFlow");
            RedisConnectionLocal = $"{HostName}:{PortNumber},password={Password}";
            RedisConnection = $"redis://{HostName}:{PortNumber}";

            Domain = configuration.GetValue<string>("Auth0:Domain") ?? string.Empty;
            Audience = configuration.GetValue<string>("Auth0:Audience") ?? string.Empty;
            Issuer = configuration.GetValue<string>("JWT:Issuer") ?? string.Empty;
            SecretKey = configuration.GetValue<string>("JWT:Key") ?? string.Empty;
            ExpiresToken = configuration.GetValue<string>("JWT:ExpiresToken") ?? string.Empty;

            ReadApp2 = configuration.GetValue<string>("READAPP:PASSWORD") ?? string.Empty;

            ReadApp3 = configuration.GetValue<string>("READAPP:HOSTNAME") ?? string.Empty;

            var possibleCountriesAllowed = configuration.GetSection("CountriesPerAreasAllowed").Get<Dictionary<string, List<string>>>();
            if (possibleCountriesAllowed != null)
            {
                CountriesAllowed = possibleCountriesAllowed;
            }

            ReadApp1 = configuration.GetSection("ARRAYAPP").Get<Dictionary<string, List<string>>>() ?? ReadApp1;
        }

        public static void AddResources(IServiceCollection services)
        {
            services.AddSingleton<IMiddlewareResource, MiddlewareResource>();
            services.AddSingleton<IRedisResources, RedisResources>();
            services.AddSingleton<ISmsResources, SmsResources>();
            services.AddSingleton<IWhatsAppResources, WhatsAppResources>();
            services.AddSingleton<IOtpResources, OtpResources>();
            services.AddSingleton<IOtpControllerResources, OtpControllerResources>();
            services.AddSingleton<ITemplateResources, TemplateResources>();
            services.AddSingleton<IEmailResources, EmailResources>();
        }

        public static void AddSingleton(IServiceCollection services)
        {
            AddRedisLocal(services);

            services.AddSingleton<IOtpManager, OtpManager>();
        }

        public static void AddSmsApi(IConfiguration configuration, IServiceCollection services)
        {
            SmsConfig.Uri = configuration.GetValue<string>("SmsProvider:Uri") ?? string.Empty;
            SmsConfig.RequestUri = configuration.GetValue<string>("SmsProvider:RequestUri") ?? string.Empty;
            SmsConfig.Encpwd = configuration.GetValue<string>("SmsProvider:Encpwd") ?? string.Empty;
            SmsConfig.Cte = configuration.GetValue<string>("SmsProvider:Account:Cte") ?? string.Empty;
            SmsConfig.Email = configuration.GetValue<string>("SmsProvider:Account:Email") ?? string.Empty;

            SmsConfig.PollyDefault.RetryCount = configuration.GetValue<int>("SmsProvider:HttpFactory:RetryCount");
            SmsConfig.PollyDefault.SleepDuration = configuration.GetValue<int>("SmsProvider:HttpFactory:SleepDuration");
            SmsConfig.PollyDefault.DuritionBreakOf = configuration.GetValue<int>("SmsProvider:HttpFactory:DuritionBreakOf");
            SmsConfig.PollyDefault.HandledEventsAllowedBeforeBreaking = configuration.GetValue<int>("SmsProvider:HttpFactory:MaxRetryCount");
            SmsConfig.PollyDefault.HandlerLifeTime = configuration.GetValue<int>("SmsProvider:HttpFactory:HandlerLifeTime");

            var possibleCountriesAllowed = configuration.GetSection("CountriesPerAreasAllowed").Get<Dictionary<string, List<string>>>();
            if (possibleCountriesAllowed != null)
            {
                CountriesAllowed = possibleCountriesAllowed;
            }

            services.AddHttpClient<ISmsManager, AuronixSmsManager>(client =>
            {
                client.BaseAddress = new Uri(SmsConfig.Uri);
            })
                .AddPolicyHandler(GetRetryPolicy(SmsConfig.PollyDefault.RetryCount, SmsConfig.PollyDefault.SleepDuration))
                .AddPolicyHandler(GetCircuitBreakerPolicy(SmsConfig.PollyDefault.HandledEventsAllowedBeforeBreaking, SmsConfig.PollyDefault.DuritionBreakOf))
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));

            // TODO: TimeSpan.FromMinutes(5) -> SmsConfig.PollyDefault.HandlerLifeTime
        }

        public static void AddWhatsAppApi(IConfiguration configuration, IServiceCollection services)
        {
            WhatsAppConfig.ApiKey = configuration.GetValue<string>("WhatsAppProvider:ApiKey") ?? string.Empty;
            WhatsAppConfig.Uri = configuration.GetValue<string>("WhatsAppProvider:Uri") ?? string.Empty;
            WhatsAppConfig.RequestUri = configuration.GetValue<string>("WhatsAppProvider:RequestUri") ?? string.Empty;
            WhatsAppConfig.Channel = configuration.GetValue<string>("WhatsAppProvider:Model:Channel") ?? string.Empty;
            WhatsAppConfig.DestinationUserId = configuration.GetValue<string>("WhatsAppProvider:Model:DestinationUserId") ?? string.Empty;
            WhatsAppConfig.TemplateId = configuration.GetValue<string>("WhatsAppProvider:Model:TemplateId") ?? string.Empty;
            WhatsAppConfig.Language = configuration.GetValue<string>("WhatsAppProvider:Model:Language") ?? string.Empty;
            WhatsAppConfig.Url = configuration.GetValue<string>("WhatsAppProvider:Model:Url") ?? string.Empty;
            WhatsAppConfig.Mime = configuration.GetValue<string>("WhatsAppProvider:Model:Mime") ?? string.Empty;
            WhatsAppConfig.BlackListIds = configuration.GetSection("WhatsAppProvider:Model:BlackListIds").Get<string[]>() ?? new string[] { };
            WhatsAppConfig.TransationMetadata = configuration.GetSection("WhatsAppProvider:Model:TransationMetadata").Get<List<TransationMetadata>>() ?? new List<TransationMetadata>();
            WhatsAppConfig.RetryCount = configuration.GetValue<int>("WhatsAppProvider:HttpFactory:RetryCount");
            WhatsAppConfig.SleepDuration = configuration.GetValue<int>("WhatsAppProvider:HttpFactory:SleepDuration");
            WhatsAppConfig.DuritionBreakOf = configuration.GetValue<int>("WhatsAppProvider:HttpFactory:DuritionBreakOf");
            WhatsAppConfig.HandledEventsAllowedBeforeBreaking = configuration.GetValue<int>("WhatsAppProvider:HttpFactory:MaxRetryCount");
            WhatsAppConfig.HandlerLifeTime = configuration.GetValue<int>("WhatsAppProvider:HttpFactory:HandlerLifeTime");
            WhatsAppConfig.Headers.Add("apikey", WhatsAppConfig.ApiKey);
            services.AddHttpClient<IWhatsAppManager, WhatsAppManager>(client =>
            {
                client.BaseAddress = new Uri(WhatsAppConfig.Uri);
            })
                .AddPolicyHandler(GetRetryPolicy(WhatsAppConfig.RetryCount, WhatsAppConfig.SleepDuration))
                .AddPolicyHandler(GetCircuitBreakerPolicy(WhatsAppConfig.HandledEventsAllowedBeforeBreaking, WhatsAppConfig.DuritionBreakOf))
                .SetHandlerLifetime(TimeSpan.FromMinutes(WhatsAppConfig.HandlerLifeTime));
        }

        public static void AddAwsServices(IConfiguration configuration, IServiceCollection services)
        {
            EmailConfig.AccessKeyId = configuration.GetValue<string>("EmailConfig:AccessKeyId") ?? string.Empty;
            EmailConfig.SecretKeyId = configuration.GetValue<string>("EmailConfig:SecretKeyId") ?? string.Empty;
            EmailConfig.Subject = configuration.GetValue<string>("EmailConfig:Subject") ?? string.Empty;
            EmailConfig.From = configuration.GetValue<string>("EmailConfig:From") ?? string.Empty;
            EmailConfig.Source = configuration.GetValue<string>("EmailConfig:From") ?? string.Empty;
            EmailConfig.SourceArn = configuration.GetValue<string>("EmailConfig:SourceArn") ?? string.Empty;
            EmailConfig.RegionEndpoint = configuration.GetValue<string>("EmailConfig:RegionEndpoint") ?? string.Empty;

            services.AddSingleton<IAmazonSimpleEmailService>(new AmazonSimpleEmailServiceClient(EmailConfig.AccessKeyId, EmailConfig.SecretKeyId, Amazon.RegionEndpoint.GetBySystemName(EmailConfig.RegionEndpoint)));
            services.AddSingleton<IEmailManager, EmailManager>();
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int handledEventsAllowedBeforeBreaking, int duritionBreakOf)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(2, TimeSpan.FromSeconds(duritionBreakOf));

            // TODO: .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking, TimeSpan.FromSeconds(duritionBreakOf));
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount, int sleepDuration)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(sleepDuration, retryAttempt)));
        }

        private static void AddRedisLocal(IServiceCollection services)
        {
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(OtpConfiguration.RedisConnectionLocal));

            services.AddSingleton<IRedisManager, RedisManager>();
        }
    }
}
