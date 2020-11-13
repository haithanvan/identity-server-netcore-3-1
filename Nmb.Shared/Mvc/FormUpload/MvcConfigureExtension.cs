using Microsoft.AspNetCore.Mvc;

namespace Nmb.Shared.Mvc.FormUpload
{
    public static class MvcConfigureExtension
    {
        public static void AllowUploadLargeFiles(this MvcOptions mvcOptions)
        {
            mvcOptions.ModelBinderProviders.Insert(0, new TempFormFileModelBinderProvider());
        }
    }
}