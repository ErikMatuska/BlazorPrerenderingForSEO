using System.Linq;
using System.Runtime.CompilerServices;
using BlazorPrerenderingForSEO.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing.Tree;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BlazorPrerenderingForSEO.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDetection();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddSession();

            services.AddTransient<IPrerenderedManagerService, CrawlerPrerenderedManagerService>();

            // For testing purposes
            //services.AddTransient<IPrerenderedManagerService, SpecificQueryPrerenderedManagerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseDetection();

            app.UseWhen((context) => context.ShouldUsePrerendering(), (builder) =>
            {
                builder.UseEndpointRouting(true);
            });

            app.UseEndpointRouting(false);
        }
    }

    public static class HttpContextExtensions
    {
        public static bool ShouldUsePrerendering(this HttpContext context)
        {
            var service = context.RequestServices.GetRequiredService<IPrerenderedManagerService>();

            return service.ShouldUsePrerendering();
        }
    }

    public static class StartupExtensions
    {
        public static IApplicationBuilder UseEndpointRouting(this IApplicationBuilder app, bool isPrerendered)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();

                if (isPrerendered)
                {
                    endpoints.MapFallbackToPage("/Prerendering/_Host");
                }
                else
                {
                    endpoints.MapFallbackToFile("index.html");
                }
            });

            return app;
        }
    }
}
