using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using NRZMyk.Services.Services;

namespace NRZMyk.Services.Validation
{
    public class SensitivityTestNotEmptyWithoutCommentAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(validationContext.ObjectInstance is SentinelEntryRequest request))
            {
                return ValidationResult.Success;
            }

            if (!(value is IEnumerable<AntimicrobialSensitivityTestRequest> sensitivityTests))
            {
                return ValidationResult.Success;
            }

            var isInvalid = !sensitivityTests.Any() && string.IsNullOrWhiteSpace(request.Remark);
            return isInvalid
                ? new ValidationResult(ErrorMessage, new[] { validationContext.MemberName })
                : ValidationResult.Success;
        }
    }
}
