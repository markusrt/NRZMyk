using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using NRZMyk.Services.Services;

namespace NRZMyk.Services.Validation
{
    public class OtherValueAttribute : ValidationAttribute
    {
        private readonly int _otherValue;
        private readonly string _enumProperty;

        public OtherValueAttribute(int otherValue, string enumProperty)
        {
            _otherValue = otherValue;
            _enumProperty = enumProperty;
        }

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            var sentinelEntry = (SentinelEntryRequest)validationContext.ObjectInstance;

            var enumProperty = sentinelEntry.GetType().GetProperty(_enumProperty);
            if (enumProperty != null && (int)enumProperty.GetValue(sentinelEntry) == _otherValue && string.IsNullOrEmpty(value as string))
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
