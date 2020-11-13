using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;

namespace Nmb.Shared.Localization
{
    public static class AddConfigureLocalizationExtensions
    {
        public static void ConfigureLocalization(this IServiceCollection services)
        {
            services.AddPortableObjectLocalization(o => o.ResourcesPath = "Localization");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo(CultureName.Vietnamese),
                    new CultureInfo(CultureName.English)
                };

                options.DefaultRequestCulture = new RequestCulture(CultureName.Vietnamese);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider(),
                    new AcceptLanguageHeaderRequestCultureProvider()
                };
            });
            services.AddLocalization();
        }
    }
}
