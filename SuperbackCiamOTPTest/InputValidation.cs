// ---------------------------------------------------------------------------------------------
// <copyright file="InputValidation.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using SuperbackCiamOTP.Managers;
using SuperbackCiamOTP.Validations;

namespace SuperbackCiamOTPTest
{
    public class InputValidation
    {
        public InputValidation()
        {
            OtpConfiguration.CountriesAllowed = new Dictionary<string, List<string>>
            {
                { "1", new List<string> { "US", "CA" } },
                { "52", new List<string> { "MX" } },
            };
        }

        public static bool ValidatePayload(object payload)
        {
            var context = new ValidationContext(payload, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(payload, context, validationResults, true);
            return isValid;
        }

        [Fact]
        public void PhoneValidator()
        {
            var attr = new IsValidPhoneAttribute();

            Assert.True(attr.IsValid("+525544551633"), "Mexico phone should be valid");
            Assert.True(attr.IsValid("+16132523889"), "USA phone should be valid");
            Assert.True(attr.IsValid("+16125532523"), "Canada phone should be valid");

            Assert.False(attr.IsValid("+447975777666"), "Valid phone from UK should be invalid");

            Assert.False(attr.IsValid("515544551633"), "Valid phone without + should be invalid");
            Assert.False(attr.IsValid("+51 5544561533"), "Valid phone with spaces should be invalid");
            Assert.False(attr.IsValid(" +51 5544561533 "), "Valid phone with trailing spaces should be invalid");
            Assert.False(attr.IsValid("+515544551633."), "Valid phone with other characters spaces should be invalid");
            Assert.False(attr.IsValid("5544561533"), "Valid phone without country code should be invalid");
        }

        [Fact]
        public void UsernameValidator()
        {
            var userAttr = new IsValidUserAttribute();
            var emailAttr = new EmailAddressAttribute();

            string validEmail = "coolemail@domain.com";
            Assert.Equal(
                emailAttr.IsValid(validEmail),
                userAttr.IsValid(validEmail));
            string invalidEmail = "nodomainemail.com.mx";
            Assert.Equal(
                emailAttr.IsValid(invalidEmail),
                userAttr.IsValid(invalidEmail));

            string validPhone = "+515544551633";
            Assert.Equal(
                emailAttr.IsValid(validPhone),
                userAttr.IsValid(validPhone));
            string invalidPhone = "5544561533";
            Assert.Equal(
                emailAttr.IsValid(invalidPhone),
                userAttr.IsValid(invalidPhone));
        }
    }
}
