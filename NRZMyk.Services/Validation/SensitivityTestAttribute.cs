using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using NRZMyk.Services.Services;

namespace NRZMyk.Services.Validation
{
    public class SensitivityTestAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is IEnumerable<AntimicrobialSensitivityTestRequest> sensitivityTests))
            {
                return ValidationResult.Success;
            }

            var isInvalid = !sensitivityTests.All(s => s.MinimumInhibitoryConcentration.HasValue);
            return isInvalid 
                ? new ValidationResult(ErrorMessage, new []{validationContext.MemberName})
                : ValidationResult.Success;
        }
    }
}
