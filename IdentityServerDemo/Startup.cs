using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Api;
using Microsoft.AspNetCore.Http;
using IdentityServerDemo.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Nmb.Shared.Identity;
using System;
using AuthenticationSettings = Nmb.Shared.Identity.AuthenticationSettings;
using IdentityServerDemo.Domain.AccountAggregate;
using IdentityServerDemo.Helpers;
using IdentityServerDemo.ExternalAuth;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using AspNet.Security.OpenIdConnect.Primitives;
using Nmb.Shared.Constants;
using IdentityServerDemo.Infrastructure;
using Microsoft.Extensions.Options;
using Nmb.Shared.Localization;
using Nmb.Shared.Mvc.FormUpload;

namespace IdentityServerDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnv)
        {
            Configuration = configuration;
            _hostingEnv = hostingEnv;
        }

        public static readonly string AssemblyName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _hostingEnv;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomDbContext(Configuration)
                    .AddIoC(Configuration);

            services
                    .AddIdentity<Account, Role>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.AddIdentityServer()
               .AddAspNetIdentity<Account>()
               .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
               .AddCertificate(Configuration, !_hostingEnv.IsProduction())
               .AddConfigurationStore(opts =>
               {
                   opts.ConfigureDbContext = co => co.UseNpgsql(Configuration.GetConnectionString(ConfigurationKeys.DefaultConnectionString), pg =>
                   {
                       pg.MigrationsAssembly(AssemblyName);
                       pg.MigrationsHistoryTable("__EFMigrationsHistory", ApplicationDbContext.SchemaName);
                   });
                   opts.DefaultSchema = ApplicationDbContext.SchemaName;
               })
               .AddOperationalStore(opts =>
               {
                   opts.ConfigureDbContext = co => co.UseNpgsql(Configuration.GetConnectionString(ConfigurationKeys.DefaultConnectionString), pg =>
                   {
                       pg.MigrationsAssembly(AssemblyName);
                       pg.MigrationsHistoryTable("__EFMigrationsHistory", ApplicationDbContext.SchemaName);
                   });
                   opts.DefaultSchema = ApplicationDbContext.SchemaName;
                   opts.EnableTokenCleanup = true;
                   opts.TokenCleanupInterval = 3600;
               })
               .AddProfileService<ProfileService>()
               .AddJwtBearerClientAuthentication();
            services.Configure<IdentityOptions>(opts =>
            {
                opts.Password.RequireDigit = true;
                opts.Password.RequireLowercase = true;
                opts.Password.RequireUppercase = true;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequiredUniqueChars = 0;
                opts.Password.RequiredLength = 8;
                opts.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                opts.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                opts.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
                opts.SignIn.RequireConfirmedEmail = false;
                opts.User.RequireUniqueEmail = true;
                opts.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                opts.Tokens.ChangeEmailTokenProvider = TokenOptions.DefaultEmailProvider;
                opts.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                opts.Lockout.MaxFailedAccessAttempts = 5;
                opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
            });
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Configuration.GetValue<string>("KeyRingPath")));
            services.AddExternalAuth();
            services.ConfigureLocalization();
            services.AddSwagger();
            services.AddMvc(opts =>
            {
                opts.AllowUploadLargeFiles();
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStaticFiles();
            // fix chrome browser,Chrome enabled a new feature "Cookies without SameSite must be secure", 
            // the cookies shoold be expided from https, but in demo project, the internal comunicacion in docker compose is http.
            // To avoid this problem, the policy of cookies shold be in Lax mode.
            // vì mày mà t mất cả ngày
            app.UseSwaggerUi();
            app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax }); // 
            app.UseCors(opts =>
            {
                opts.AllowAnyHeader();
                opts.AllowAnyMethod();
                opts.AllowCredentials();
                opts.SetIsOriginAllowed(origin => true);
            });
            app.UseSession();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
