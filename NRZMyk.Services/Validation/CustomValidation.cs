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
        private readonly string _otherProperty;

        public OtherValueAttribute(int otherValue, string otherProperty)
        {
            _otherValue = otherValue;
            _otherProperty = otherProperty;
        }

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            var sentinelEntry = (SentinelEntryRequest)validationContext.ObjectInstance;

            var otherProperty = sentinelEntry.GetType().GetProperty(_otherProperty);
            if ((int)sentinelEntry.Material == _otherValue && string.IsNullOrEmpty(otherProperty?.GetValue(sentinelEntry) as string))
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
