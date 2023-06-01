// ---------------------------------------------------------------------------------------------
// <copyright file="ISerializador.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System;

namespace SuperbackCiamOTP.Interfaces
{
    public interface ISerializador
    {
        public string Serialize(string message, object classsToSerialize);
    }
}
