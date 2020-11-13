using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nmb.Shared.Mvc.FormUpload
{
    public class LargeFileValueProvider : BindingSourceValueProvider, IValueProvider
    {
        private static readonly FormOptions DefaultFormOptions = new FormOptions();
        private Dictionary<string, StringValues> _values;

        public LargeFileValueProvider(Dictionary<string, StringValues> values) : base(BindingSource.Form)
        {
            _values = values;
        }

        public override bool ContainsPrefix(string prefix)
        {
            return _values.ContainsKey(prefix);
        }

        public override ValueProviderResult GetValue(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof (key));
            }

            return _values.TryGetValue(key, out var value) ? new ValueProviderResult(value) : ValueProviderResult.None;
        }

    }

    public class LargeFileValueProviderFactory : IValueProviderFactory
    {
        public async Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            var logger =
                context.ActionContext.HttpContext.RequestServices.GetService<ILogger<LargeFileValueProviderFactory>>();
            var (files, formItems) = await context.ActionContext.HttpContext.Request.ParseFormAsync(logger);

            foreach (var item in files)
            {
                formItems.Append(item.Name, item.Name);
                foreach (var (key, value) in item.GetValues())
                {
                    formItems.Append(key, value);
                }
            }

            context.ValueProviders.Add(new LargeFileValueProvider(formItems.GetResults()));
        }
    }
}
