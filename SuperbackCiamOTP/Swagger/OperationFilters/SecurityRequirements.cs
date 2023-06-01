// ---------------------------------------------------------------------------------------------
// <copyright file="SecurityRequirements.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------
using System;
using Microsoft.OpenApi.Models;
using SuperbackCiamOTP.Swagger.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SuperbackCiamOTP.Swagger.OperationFilters
{
    public class SecurityRequirements : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var requiresAuthorization = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<RequiresAuthorizationAttribute>().FirstOrDefault();

            if (requiresAuthorization == null)
            {
                return;
            }

            // If it has any authorization requirement we add Unauthorized to the response types
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });

            // Add the security scheme corresponding to the SecuritySchemeId
            if (requiresAuthorization.SchemeId == SecuritySchemeIds.Ciam)
            {
                if (requiresAuthorization.Scopes.Any())
                {
                    operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
                }

                var oAuthScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = SecuritySchemeIds.Ciam },
                };
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [oAuthScheme] = requiresAuthorization.Scopes,
                    },
                };
            }
            else if (requiresAuthorization.SchemeId == SecuritySchemeIds.Otp)
            {
                if (requiresAuthorization.Scopes.Any())
                {
                    operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
                }

                var otpScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = SecuritySchemeIds.Otp },
                };

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [otpScheme] = requiresAuthorization.Scopes,
                    },
                };
            }
        }
    }
}
