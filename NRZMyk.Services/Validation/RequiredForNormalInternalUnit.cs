using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;

namespace NRZMyk.Services.Validation
{
    public class RequiredForNormalInternalUnit : ValidationAttribute
    {
        public RequiredForNormalInternalUnit() 
            : base("Der Wert für muss für die internistische Normalstation angegeben werden.")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var isInvalid = false;
            if (validationContext is { ObjectInstance: SentinelEntryRequest request })
            {
                var isInternalNormalUnit = request.HospitalDepartment == HospitalDepartment.Internal
                        && request.HospitalDepartmentType == HospitalDepartmentType.NormalUnit;
                var internalTypeSetToNone = request.InternalHospitalDepartmentType == InternalHospitalDepartmentType.NoInternalDepartment;
                var missingForNormalUnit = isInternalNormalUnit && internalTypeSetToNone;
                var setForNonNormalUnit = !isInternalNormalUnit && !internalTypeSetToNone;
                
                isInvalid = missingForNormalUnit | setForNonNormalUnit;
            }

            return isInvalid 
                ? new ValidationResult(ErrorMessage, new []{validationContext.MemberName})
                : ValidationResult.Success;
        }
    }
}
