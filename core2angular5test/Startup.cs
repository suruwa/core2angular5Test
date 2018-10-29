using core2angular5test.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace core2angular5test
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
            //Wyciąganie ustawień:
            //var myValue = configuration["Logging:IncludeScopes"];        
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // and configure dependency injection
        public void ConfigureServices(IServiceCollection services)
        {
            //Registering MVC using the Dependency Injection framework built into
            //ASP.NET Core
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //Add EntityFramework support for Postgres
            services.AddEntityFrameworkNpgsql();
            
            //Add ApplicationDbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //Dlaczego nie użyte
                //app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions()
                //{
                //    HotModuleReplacement = true
                //});
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions() {
                OnPrepareResponse = (context) =>
                {
                    // Disable caching for all static files.
                    context.Context.Response.Headers["Cache-Control"] = Configuration["StaticFiles:Headers:Cache-Control"];
                    context.Context.Response.Headers["Pragma"] = Configuration["StaticFiles:Headers:Pragma"];
                    context.Context.Response.Headers["Expires"] = Configuration["StaticFiles:Headers:Expires"];
                    //TODO: POCO configuration retrieval options
                    /*
                     * https:/​/​weblog.​west-​wind.​com/​posts/​2016/​may/​23/​strongly-​typed-​configuration- settings-​in-​aspnet-​core
                     * https:/​/​www.​strathweb.​com/​2016/​09/​strongly-​typed-​configuration-​in-​asp-​net-​core- without-​ioptionst/​
                     * https:/​/​rimdev.​io/​strongly-​typed-​configuration-​settings-​in-​asp-​net-​core-​part- ii/​
                     */
                    
                } 
            });//Pozwala na pobieranie rzeczy z wwwroot
            app.UseSpaStaticFiles();

            //Adding the required middleware to the HTTP request pipeline, while also
            //(optionally) setting a pack of default routes
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");

                //TODO: Nieużywane w książce, dlaczego?
                //routes.MapSpaFallbackRoute(
                //    name: "spa-fallback",
                //    defaults: new { controller = "Home", action = "Index" });
            });


            //TODO: jak się używa spaservices?
            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}