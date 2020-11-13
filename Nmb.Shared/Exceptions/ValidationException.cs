using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace Nmb.Shared.Exceptions
{
    public class ValidationException : Exception
    {
        public IList<ValidationError> Errors { get; set; }

        public ValidationException()
        {
        }

        public ValidationException(ValidationError error)
        {
            Errors = new List<ValidationError> { error };
        }

        public ValidationException(string field, string message) : this(new ValidationError(field, message))
        {
        }

        public ValidationException(IEnumerable<ValidationError> errors)
        {
            Errors = errors.ToList();
        }

        public ModelStateDictionary GetModelState(IStringLocalizer localizer)
        {
            var state = new ModelStateDictionary();
            foreach (var error in Errors)
            {
                state.AddModelError(error.Field, localizer[error.Message]);
            }

            return state;
        }
    }
}
