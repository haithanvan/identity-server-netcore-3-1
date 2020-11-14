using Hangfire;
using Hangfire.Common;
using Hangfire.PostgreSql;
using IdentityServerDemo.Filters;
using IdentityServerDemo.Infrastructure;
using IdentityServerDemo.Initializations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Nmb.Shared.Constants;
using Nmb.Shared.Identity;
using Nmb.Shared.Initialization;
using Nmb.Shared.Mvc;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace IdentityServerDemo.Helpers
{
    public static class StartupHelper
    {
        public static readonly string AssemblyName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseLazyLoadingProxies()
                        .UseNpgsql(configuration.GetConnectionString(ConfigurationKeys.DefaultConnectionString), b =>
                        {
                            b.MigrationsAssembly(ConfigurationKeys.MigrationAssembly);
                            b.MigrationsHistoryTable("__EFMigrationsHistory", ApplicationDbContext.SchemaName);
                        }))
                .AddScoped<IDbConnection>(sp =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    var connection = new NpgsqlConnection(config.GetConnectionString(ConfigurationKeys.DefaultConnectionString));
                    connection.Open();
                    return connection;
                });
            return services;
        }

        public static IIdentityServerBuilder AddCertificate(this IIdentityServerBuilder builder, IConfiguration configuration, bool isDevelopment)
        {
            if (isDevelopment)
            {
                builder.AddDeveloperSigningCredential();
                return builder;
            }

            var keyFilePath = configuration.GetValue<string>(IdentityConfigurationKeys.CertFilePath);
            var keyFilePassword = configuration.GetValue<string>(IdentityConfigurationKeys.CertPassword);

            if (File.Exists(keyFilePath))
            {
                builder.AddSigningCredential(new X509Certificate2(keyFilePath, keyFilePassword));
            }
            else
            {
                Console.WriteLine($"SigninCredentialExtension cannot find key file {keyFilePath}");
            }

            return builder;
        }

        public static IServiceCollection AddIoC(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTokenRetriever(configuration)
                .AddHttpClientAuthorizationDelegatingHandler();

            //services.AddHttpClient<IUserService, UserService>()
            //    .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IInitializationStage, MigrateDatabaseInitialization>();
            services.AddTransient<IInitializationStage, SeedRoleInitialization>();
            services.AddTransient<IInitializationStage, SeedClientInitialization>();
            services.AddScoped<IScopeContext, SystemContext>();
            return services;
        }
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    var settings = configuration.GetSection("Authentication").Get<AuthenticationSettings>();
                    options.Authority = settings.Authority;
                    options.RequireHttpsMetadata = settings.RequireHttpsMetadata;
                    options.ApiName = settings.ApiName;
                    options.ApiSecret = settings.ApiSecret;
                    options.TokenRetriever = CustomTokenRetriever.FromHeaderAndQueryString();
                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromHours(1);
                });

            services.AddAuthorization(opts =>
            {
                opts.AddPolicy(NmbPolicy.Administrator, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(AllRoles.Administrator);
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                });
                opts.AddPolicy(NmbPolicy.Mobile, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(AllRoles.Mobile);
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                });
            });

            return services;
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity API", Version = "v1" });

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", securitySchema);
                var securityRequirement = new OpenApiSecurityRequirement { { securitySchema, new[] { "Bearer" } } };
                c.AddSecurityRequirement(securityRequirement);
            });
        }

        public static void UseSwaggerUi(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity API v1");
                c.DocumentTitle = "Identity API";
            });
        }

        public static IServiceCollection AddHangfireService(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(ConfigurationKeys.ScheduledTasksDbConnectionString);
            services.AddHangfire(config =>
            {
                config.UsePostgreSqlStorage(connectionString, new PostgreSqlStorageOptions()
                {
                    SchemaName = "hangfire",
                    PrepareSchemaIfNecessary = true
                });
            });

            services.AddSingleton<IBackgroundJobClient>((x =>
                new BackgroundJobClient(x.GetRequiredService<JobStorage>(),
                    x.GetRequiredService<IJobFilterProvider>())));
            return services;
        }

        public static IApplicationBuilder UseHangfire(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new HangfireAuthorizationFilter() },
                IgnoreAntiforgeryToken = true
            });
            return app;
        }
    }
}
