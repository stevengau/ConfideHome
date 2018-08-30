using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoIdentitySample.Mvc.Models;
using MongoIdentitySample.Mvc.Services;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace MongoIdentitySample.Mvc
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            var mongoSettings = Configuration.GetSection(nameof(MongoDbSettings));
            var settings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

            services.AddSingleton<MongoDbSettings>(settings);
            services.AddIdentity<ApplicationUser, MongoIdentityRole>()
                    .AddMongoDbStores<ApplicationUser, MongoIdentityRole, Guid>(settings.ConnectionString, settings.DatabaseName)
                    .AddDefaultTokenProviders();

            services.AddMvc();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
               .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
               .AddDataAnnotationsLocalization();


            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("zh-Hans"),
                };

                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders = new[]{ new ConfideHome.Override.RouteDataRequestCultureProvider{
                    IndexOfCulture=1,
                    IndexofUICulture=1
                }};
                //options.RequestCultureProviders = new[]{ new QueryStringRequestCultureProvider
                //{
                //    QueryStringKey = "culture",
                //    UIQueryStringKey = "culture"
                //} };
            });


            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("culture", typeof(LanguageRouteConstraint));
            });
            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.Use((context, next) =>
            {
                var cultureQuery = context.Request.Query["lang"];
                if (!string.IsNullOrWhiteSpace(cultureQuery))
                {
                    if (cultureQuery == "zh")
                    {
                        var newCulturedPath = context.Request.Path.ToString().Replace("/en", "/zh");
                        context.Response.Redirect(newCulturedPath);
                        return next();
                    }
                    if (cultureQuery == "en")
                    {
                        var newCulturedPath = context.Request.Path.ToString().Replace("/zh", "/en");
                        context.Response.Redirect(newCulturedPath);
                        return next();
                    }
                }
                return next();
            });
            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                         name: "LocalizedDefault",
                         template: "{culture:culture}/{controller=Home}/{action=Index}/{id?}"
                 );
                routes.MapRoute(
                         name: "map",
                         template: "{culture:culture}/{controller=Map}/{action=ForLease}/{searchCity?}/{searchDays?}/{searchBeds?}/{searchBaths?}/{searchPriceStart?}/{searchPriceEnd?}/{searchSouth?}/{searchWest?}/{searchNorth?}/{searchEast?}/{searchZoom?}"
                 );
                routes.MapRoute(
                       name: "default",
                       template: "{*catchall}",
                       defaults: new { controller = "Home", action = "RedirectToDefaultCulture", culture = "en" });
            });
        }


    }

    public class LanguageRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {

            if (!values.ContainsKey("culture"))
                return false;

            var culture = values["culture"].ToString();
            return culture == "en" || culture == "zh";
        }
    }
}
