using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Authorization.DependencyResolution.Microsoft;
using SFA.DAS.Authorization.Mvc.Extensions;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Startup;
using SFA.DAS.Roatp.CourseManagement.Application.Providers.Queries;
using SFA.DAS.Roatp.CourseManagement.Domain.Configuration;
using SFA.DAS.Roatp.CourseManagement.Web.AppStart;
using SFA.DAS.Roatp.CourseManagement.Web.HealthCheck;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;

namespace SFA.DAS.Roatp.CourseManagement.Web
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;
        private static readonly string[] tags = new[] { "ready" };

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

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddConfigurationOptions(_configuration);

            services.AddSingleton<IAuthorizationHandler, ProviderAuthorizationHandler>();
            services.AddSingleton<ITrainingProviderAuthorizationHandler, TrainingProviderAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, TrainingProviderAllRolesAuthorizationHandler>();

            services.AddProviderUiServiceRegistration(_configuration);

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(GetProviderQuery).Assembly));

            services.AddAuthorizationServicePolicies();

            if (_configuration["StubProviderAuth"] != null && _configuration["StubProviderAuth"].Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddProviderStubAuthentication();
            }
            else
            {
                services.AddAndConfigureProviderAuthentication(_configuration);
            }

            services.Configure<IISServerOptions>(options => { options.AutomaticAuthentication = false; });

            services.AddAuthorization<AuthorizationContextProvider>();
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
                .AddSessionStateTempDataProvider()
                .SetDefaultNavigationSection(NavigationSection.StandardsAndTrainingVenues)
                .ShowBetaPhaseBanner()
                .EnableGoogleAnalytics();
            /// .SetZenDeskConfiguration(_configuration.GetSection("ProviderZenDeskSettings").Get<ZenDeskConfiguration>());

            services
            .AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters()
            .AddValidatorsFromAssemblyContaining<Startup>();

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

            services.AddHealthChecks()
                    .AddCheck<CourseManagementOuterApiHealthCheck>(CourseManagementOuterApiHealthCheck.HealthCheckResultDescription,
                        failureStatus: HealthStatus.Unhealthy,
                        tags: tags);
            services.AddDataProtection(_configuration);

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.IsEssential = true;
            });

            services.AddApplicationInsightsTelemetry();

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

                context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");

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
            app.UseHealthChecks();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
