using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.DependencyResolution.Microsoft;
using SFA.DAS.Authorization.Mvc.Extensions;
using SFA.DAS.Authorization.ProviderFeatures.Configuration;
using SFA.DAS.Authorization.ProviderFeatures.DependencyResolution.Microsoft;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Startup;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.Configuration;
using SFA.DAS.Roatp.CourseManagement.Web.AppStart;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace SFA.DAS.Roatp.CourseManagement.Web
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        public Startup(IConfiguration configuration)
        {
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory());
#if DEBUG
            if (!configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                config.AddJsonFile("appsettings.json", true)
                    .AddJsonFile("appsettings.Development.json", true);
            }
#endif
            config.AddEnvironmentVariables();

            if (!configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                config.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                }
                );
            }

            _configuration = config.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var roatpCourseManagementConfiguration = _configuration
                .GetSection(nameof(RoatpCourseManagement))
                .Get<RoatpCourseManagement>();

            var providerFeaturesConfiguration = _configuration
                .GetSection(nameof(ProviderFeaturesConfiguration))
                .Get<ProviderFeaturesConfiguration>();

            services.AddSingleton(providerFeaturesConfiguration);

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddConfigurationOptions(_configuration);

            services.AddSingleton<IAuthorizationHandler, ProviderAuthorizationHandler>();

            services.AddProviderUiServiceRegistration(_configuration);

            services.AddMediatR(typeof(GetAllProviderStandardsQueryHandler).Assembly);

            services.AddAuthorizationServicePolicies();

            if (_configuration["StubProviderAuth"] != null && _configuration["StubProviderAuth"].Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddProviderStubAuthentication();
            }
            else
            {
                var providerConfig = _configuration.GetSection(nameof(ProviderIdams)).Get<ProviderIdams>();
                services.AddAndConfigureProviderAuthentication(providerConfig);
            }

            services.Configure<IISServerOptions>(options => { options.AutomaticAuthentication = false; });

            services.AddAuthorization<AuthorizationContextProvider>();
            services.AddProviderFeaturesAuthorization();

            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            }).AddMvc(options =>
            {
                options.AddAuthorization();
                if (!_configuration.IsDev())
                {
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                }
            })
            .SetDefaultNavigationSection(NavigationSection.Home)
            /// .EnableGoogleAnalytics()
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            /// .SetZenDeskConfiguration(_configuration.GetSection("ProviderZenDeskSettings").Get<ZenDeskConfiguration>());
            .AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining<Startup>();
                fv.ImplicitlyValidateChildProperties = true;
            });

            services.AddHttpContextAccessor();

            if (_configuration.IsDev() || _configuration.IsLocal())
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = roatpCourseManagementConfiguration.RedisConnectionString;
                });
            }

            services.AddHealthChecks();
            services.AddDataProtection(_configuration);

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.IsEssential = true;
            });

            services.AddApplicationInsightsTelemetry(_configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

            services.AddLogging();

            services.AddServiceRegistrations(_configuration);
            
#if DEBUG
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
#endif
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHealthChecks();
                app.UseStatusCodePagesWithReExecute("/error/{0}");
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.Use(async (context, next) =>
            {
                if (context.Response.Headers.ContainsKey("X-Frame-Options"))
                {
                    context.Response.Headers.Remove("X-Frame-Options");
                }

                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");

                await next();

                if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
                {
                    //Re-execute the request so the user gets the error page
                    var originalPath = context.Request.Path.Value;
                    context.Items["originalPath"] = originalPath;
                    context.Request.Path = "/error/404";
                    await next();
                }
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
