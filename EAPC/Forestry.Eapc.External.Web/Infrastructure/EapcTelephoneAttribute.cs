using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Forestry.Eapc.External.Web.Infrastructure
{
    /// <summary>
    /// EAPC-specific validation for what makes a valid telephone number.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class EapcTelephoneAttribute : DataTypeAttribute
    {
        /// <inheritdoc />
        public EapcTelephoneAttribute() : base(DataType.PhoneNumber)
        {
            
        }

        /// <inheritdoc />
        public override bool IsValid(object? value)
        {
            return IsValidTelephoneNumber(value as string);
        }

        public static bool IsValidTelephoneNumber(string? value)
        {
            if (value == null) return true; // this should be handled by other validation

            value = value.Trim();
            var expectedNumberOfDigits = 11;

            if (value.StartsWith('+'))
            {
                value = value.Substring(1);
                expectedNumberOfDigits = 12;
            }

            var chars = value.ToCharArray().Where(c => c != ' '); // get all the non-space characters
            var count = chars.Count(char.IsNumber);
            return count == expectedNumberOfDigits;
        }
    }
}
