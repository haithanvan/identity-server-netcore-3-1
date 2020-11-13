using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Nmb.Shared.Mvc.FormUpload
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LargeFileModelBindingAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var factories = context.ValueProviderFactories;
            factories.RemoveType<FormValueProviderFactory>();
            factories.RemoveType<JQueryFormValueProviderFactory>();
            //factories.Insert(0, new LargeFileValueProviderFactory());
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}
