using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nmb.Shared.Mvc.FormUpload
{
    public class TempFormFileModelBinder : IModelBinder
    {
        private readonly ILogger<TempFormFileModelBinder> _logger;

        public TempFormFileModelBinder(ILoggerFactory logger)
        {
            _logger = logger.CreateLogger<TempFormFileModelBinder>();
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var files = await ReadFormFile(bindingContext);
            var modelName = bindingContext.ModelName;

            if (bindingContext.ModelType == typeof(TempFormFile))
            {
                if (files.Count == 0)
                {
                    return;
                }

                var obj = files.First();
                bindingContext.ValidationState.Add(obj, new ValidationStateEntry()
                {
                    Key = modelName
                });
                bindingContext.ModelState.SetModelValue(modelName, null, null);
                bindingContext.Result = ModelBindingResult.Success(obj);
            }

            if (typeof(IEnumerable<TempFormFile>).IsAssignableFrom(bindingContext.ModelType))
            {
                bindingContext.ValidationState.Add(files, new ValidationStateEntry()
                {
                    Key = modelName
                });
                bindingContext.ModelState.SetModelValue(modelName, null, null);
                bindingContext.Result = ModelBindingResult.Success(files);
            }
        }

        private async Task<ICollection<TempFormFile>> ReadFormFile(ModelBindingContext bindingContext)
        {
            var context = bindingContext.HttpContext;
            var logger = context.RequestServices.GetService<ILogger<TempFormFileModelBinder>>();
            var (files, formItems) = await context.Request.ParseFormAsync(logger);
            return files;
        }
    }

    public class TempFormFileModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var modelType = context.Metadata.ModelType;
            if (modelType != typeof(TempFormFile) && !typeof(IEnumerable<TempFormFile>).IsAssignableFrom(modelType))
            {
                return (IModelBinder)null;
            }

            var requiredService = context.Services.GetRequiredService<ILoggerFactory>();
            return (IModelBinder)new TempFormFileModelBinder(requiredService);
        }
    }
}
