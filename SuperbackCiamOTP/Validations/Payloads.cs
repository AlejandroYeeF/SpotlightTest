// ---------------------------------------------------------------------------------------------
// <copyright file="Payloads.cs" company="DigitalFEMSA">
// Copyright (c) DigitalFEMSA. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using SuperbackCiamOTP.Entities;
using SuperbackCiamOTP.Managers;

namespace SuperbackCiamOTP.Validations
{
    /// <summary>
    /// Class to check if a number is valid and generic error messages for payload validations
    /// </summary>
    internal static partial class GenericMessages
    {
        public const string PhoneInvalid = "The phone number field is not a valid phone number.";
        public const string PhoneOutCoverage = "The phone number is out of coverage.";
        public const string UserInvalid = "The username field is not a valid phone or email.";
        public const string PhoneNumberAndEmail = "You cannot send phone number and email in the same request.";
        public const string NotPhoneNumberAndEmail = "The phone number or mail must be filled.";

        // Regex to check that content of PhoneNumber value are digits
        [GeneratedRegex("^[0-9]+$")]
        private static partial Regex PhoneNumberRegex();

        public static bool ValidNumber(string phoneUser)
        {
            return GenericMessages.CheckStructure(phoneUser) && GenericMessages.CheckCountry(phoneUser);
        }

        public static bool CheckStructure(string phoneUser)
        {
            // Check minimum length
            if (phoneUser.Length <= 10 || phoneUser[0] != '+')
            {
                return false;
            }

            // Check only digits
            if (!PhoneNumberRegex().IsMatch(phoneUser[1..]))
            {
                return false;
            }

            return true;
        }

        public static bool CheckCountry(string phoneUser)
        {
            // Get area code (all internal numbers are ten digits length)
            // TODO: Extend to numbers with less than ten digits (e.g. Chile)
            int newPosition = phoneUser.Length - 10;
            string strArea = phoneUser[..newPosition].Replace("+", string.Empty);

            // Check if area code is valid
            var countriesAllowed = OtpConfiguration.CountriesAllowed;
            return countriesAllowed.ContainsKey(strArea);
        }
    }

    public class IsValidPhoneNumberAndEmailAttribute : ValidationAttribute
    {
        /// <inheritdoc/>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is UserMetaData user)
            {
                if (!string.IsNullOrEmpty(user.PhoneNumber) && !string.IsNullOrEmpty(user.Email))
                {
                    throw new Exception(GenericMessages.PhoneNumberAndEmail);
                }

                if (string.IsNullOrEmpty(user.PhoneNumber) && string.IsNullOrEmpty(user.Email))
                {
                    throw new Exception(GenericMessages.NotPhoneNumberAndEmail);
                }
            }

            return ValidationResult.Success;
        }
    }

    public class IsValidPhoneAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is not string || !GenericMessages.ValidNumber((string)value))
            {
                if (GenericMessages.CheckStructure((string)value) && !GenericMessages.CheckCountry((string)value))
                {
                    return new ValidationResult(GenericMessages.PhoneOutCoverage);
                }

                return new ValidationResult(GenericMessages.PhoneInvalid);
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }

    public class IsValidUserAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string)
            {
                return new ValidationResult(GenericMessages.UserInvalid);
            }

            var userNameMail = (string)value;

            if (userNameMail.IndexOf("@") > 0)
            {
                if (new EmailAddressAttribute().IsValid(userNameMail))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(GenericMessages.UserInvalid);
                }
            }
            else
            {
                if (!GenericMessages.ValidNumber((string)value))
                {
                    return new ValidationResult(GenericMessages.UserInvalid);
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
        }
    }

    public class DoNotSerialize : Attribute
    {
    }
}
