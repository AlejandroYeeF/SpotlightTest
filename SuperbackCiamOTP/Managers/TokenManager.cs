// ---------------------------------------------------------------------------------------------
// <copyright file="TokenManager.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

namespace SuperbackCiamOTP.Managers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.IdentityModel.Tokens;
    using SuperbackCiamOTP.Controllers;
    using SuperbackCiamOTP.Entities;
    using static SuperbackCiamOTP.Managers.StaticLoggerFactory;

    public class TokenManager
    {
        private static ILogger Logger { get => GetStaticLogger<TokenManager>(); }

        public static string GetInternalToken(RequestJwt request)
        {
            var claims = new[]
            {
                new Claim("ServiceCode", request.ServiceCode ?? string.Empty),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(OtpConfiguration.SecretKey));
            Logger.LogInformation($"JWT:Key={OtpConfiguration.SecretKey}");
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("sub", request.Username ?? string.Empty),
                        new Claim("scope", request.ServiceCode ?? string.Empty),
                    }),
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(OtpConfiguration.ExpiresToken)),
                Issuer = OtpConfiguration.Issuer,
                Audience = OtpConfiguration.Audience,
                SigningCredentials = cred,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            Logger.LogInformation($"Token:{securityToken}");
            return tokenHandler.WriteToken(securityToken);
        }

        public static bool ValidToken(string token)
        {
            var secretKey = OtpConfiguration.SecretKey;
            var securityKey = new SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(secretKey));

            SecurityToken securityToken;
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidIssuer = OtpConfiguration.Issuer,
                ValidAudience = OtpConfiguration.Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                LifetimeValidator = LifetimeValidator,
                IssuerSigningKey = securityKey,
            };

            // COMPRUEBA LA VALIDEZ DEL TOKEN
            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError($"Error validation token: {e.Message}");
                return false;
            }
        }

        public static bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            var valid = false;

            if ((expires.HasValue && DateTime.UtcNow < expires)
                && (notBefore.HasValue && DateTime.UtcNow > notBefore))
            {
                valid = true;
            }

            return valid;
        }
    }
}
