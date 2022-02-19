using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using NRZMyk.Services.Services;

namespace NRZMyk.Services.Validation
{
    public class NotTooOldAndNotInFutureAttribute : ValidationAttribute
    {
        
        private readonly int _maxAgeInYears;

        public NotTooOldAndNotInFutureAttribute(int maxAgeInYears)
        {
            if (maxAgeInYears <= 0)
            {
                throw new ArgumentException("Validation parameter must not be less or equal then zero",
                    nameof(maxAgeInYears));
            }
            _maxAgeInYears = maxAgeInYears;
            ErrorMessage =  
                "Der Wert von {0} darf nicht älter als " + _maxAgeInYears + " Jahre sein und nicht in der Zukunft liegen.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var isInvalid = false;
            if (value is DateTime date)
            {
                isInvalid = date > DateTime.Now || date < DateTime.Now.AddYears(-_maxAgeInYears);
            }
            return isInvalid 
                ? new ValidationResult(string.Format(ErrorMessage, validationContext.MemberName), new []{validationContext.MemberName})
                : ValidationResult.Success;
        }
    }
}
