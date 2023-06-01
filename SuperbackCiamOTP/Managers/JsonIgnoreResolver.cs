// ---------------------------------------------------------------------------------------------
// <copyright file="JsonIgnoreResolver.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SuperbackCiamOTP.Validations;

namespace SuperbackCiamOTP.Managers
{
    public class JsonIgnoreResolver : DefaultContractResolver
    {
        /// <inheritdoc/>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (member.GetCustomAttribute<DoNotSerialize>() != null)
            {
                property.ShouldSerialize = _ => false;
            }

            return property;
        }
    }
}
