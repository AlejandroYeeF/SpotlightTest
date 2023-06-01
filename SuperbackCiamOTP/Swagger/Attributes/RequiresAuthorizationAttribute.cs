// ---------------------------------------------------------------------------------------------
// <copyright file="RequiresAuthorizationAttribute.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------
using System;

namespace SuperbackCiamOTP.Swagger.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class RequiresAuthorizationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiresAuthorizationAttribute"/> class.
        /// </summary>
        /// <param name="schemeId">Authorization scheme identifier</param>
        /// <param name="scopes">Scope of authorization</param>
        public RequiresAuthorizationAttribute(string schemeId, string scopes = "")
        {
            this.SchemeId = schemeId;

            if (!string.IsNullOrEmpty(scopes))
            {
                this.Scopes = scopes.Split(",").ToList();
            }
        }

        public string SchemeId { get; set; }

        public List<string> Scopes { get; set; } = new List<string>();
    }

    public class SecuritySchemeIds
    {
        public const string Ciam = "ciam";
        public const string Otp = "otp";
    }

    public class SecurityScopes
    {
        public const string Scopelvl2 = "access:lvl2";
    }
}
