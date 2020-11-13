using System;
using System.ComponentModel.DataAnnotations;

namespace Nmb.Shared.Mvc
{
    public class GuidRequiredAttribute: ValidationAttribute
    {
        public string GetErrorMessage() => $"The value is required";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var guid = (Guid) value;
            return guid == Guid.Empty ? new ValidationResult(GetErrorMessage()) : ValidationResult.Success;
        }
    }
}
