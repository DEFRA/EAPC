using Forestry.Eapc.External.Web.Configuration;
using Forestry.Eapc.External.Web.Infrastructure;
using Forestry.Eapc.External.Web.Infrastructure.SecurityHeaders;
using Forestry.Eapc.External.Web.Infrastructure.TestUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Forestry.Eapc.External.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            HostEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(x =>
            {
                x.CheckConsentNeeded = _ => true;
                x.Secure = CookieSecurePolicy.Always;
            });

            // The TempData provider cookie is not essential. Make it essential
            // so TempData is functional when tracking is disabled.
            services.Configure<CookieTempDataProviderOptions>(options => {
                options.Cookie.IsEssential = true;
            }); 

            var builder = services.AddControllersWithViews();
      
            //todo remove this an the related nuget package once GDS styling work is complete.
            if (!HostEnvironment.IsProduction())
            {
                builder.AddRazorRuntimeCompilation();
            }
            
            services.AddHttpContextAccessor();
            services.AddAzureAdB2CServices(Configuration);
            services.AddEapcServices(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStatusCodePagesWithRedirects("/Home/Error");
            app.UseHttpsRedirection();

            app.UseSecurityHeadersMiddleware(new SecurityHeadersBuilder().AddDefaultSecurePolicy());

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            if (EnableTestUserMiddleware())
            {
                app.UseTestUser();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private bool EnableTestUserMiddleware()
        {
            var settings = new EnvironmentOptions();
            Configuration.Bind("Environment", settings);

            return settings.EnableTestUserMiddleware;
        }
    }
}
