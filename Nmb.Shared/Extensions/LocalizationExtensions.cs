using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;

namespace Nmb.Shared.Extensions
{
    public static class LocalizationExtensions
    {
        public static IDictionary<string, string[]> TranslateErrors(this IStringLocalizer localizer, ModelStateDictionary modelStateDictionary)
        {

            var allErrors = new Dictionary<string, string[]>(StringComparer.Ordinal);
            foreach (var (key, value) in modelStateDictionary)
            {
                var errors = value.Errors;
                if (errors == null || errors.Count <= 0)
                    continue;

                if (errors.Count == 1)
                {
                    var errorMessage = GetErrorMessage(errors[0]);
                    allErrors.Add(key, new[] { errorMessage });
                }
                else
                {
                    var errorMessages = new string[errors.Count];
                    for (var i = 0; i < errors.Count; i++)
                    {
                        errorMessages[i] = GetErrorMessage(errors[i]);
                    }

                    allErrors.Add(key, errorMessages);
                }
            }

            string GetErrorMessage(ModelError error)
            {
                return string.IsNullOrEmpty(error.ErrorMessage) ? localizer["default error message"] : localizer[error.ErrorMessage];
            }

            return allErrors;
        }
    }
}
