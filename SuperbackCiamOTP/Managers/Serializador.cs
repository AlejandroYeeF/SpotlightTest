// ---------------------------------------------------------------------------------------------
// <copyright file="Serializador.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperbackCiamOTP.Interfaces;
using SuperbackCiamOTP.Managers;

namespace SuperbackCiamOTP.Entities
{
    public class Serializador : ISerializador
    {
        public string Serialize(string message, object classsToSerialize)
        {
            return message + JsonConvert.SerializeObject(classsToSerialize, new JsonSerializerSettings { ContractResolver = new JsonIgnoreResolver() });
        }
    }
}
