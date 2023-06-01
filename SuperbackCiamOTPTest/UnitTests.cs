// ---------------------------------------------------------------------------------------------
// <copyright file="UnitTests.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using SuperbackCiamOTP;

namespace SuperbackCiamOTPTest
{
    public class UnitTests
    {
        [Fact]
        public void CheckTemperature()
        {
            var celsius = new WeatherForecast();
            celsius.TemperatureC = 30;
            Assert.True(celsius.TemperatureF == 85, "Incorrect C° to F°");
        }
    }
}
