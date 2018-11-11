using System;
using System.Text;
using core2angular5test.Data;
using core2angular5test.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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
                options
                    .UseNpgsql(Configuration.GetConnectionString("DefaultConnection"))
                    //.EnableSensitiveDataLogging()
                );
            
            // Add ASP .NET Identity support
            // https://github.com/aspnet/Identity/blob/95205d99944d6f512b61ac3d508e92662fcd3169/src/Identity/IdentityServiceCollectionExtensions.cs
            services.AddIdentity<ApplicationUser, IdentityRole>(
                opts =>
                {
                    opts.Password.RequireDigit = true;
                    opts.Password.RequireLowercase = true;
                    opts.Password.RequireUppercase = true;
                    opts.Password.RequireNonAlphanumeric = false;
                    opts.Password.RequiredLength = 7;
                }
            ).AddEntityFrameworkStores<ApplicationDbContext>();

            // Add authentication with JWT tokens
            services.AddAuthentication(opts =>
            {
                opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;                
            })
            .AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    // standard configuration
                    ValidIssuer = Configuration["Auth:Jwt:Issuer"],
                    ValidAudience = Configuration["Auth:Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["Auth:Jwt:Key"])),
                    ClockSkew = TimeSpan.Zero,

                    // security switches, can be enabled/disabled during debugging
                    // if token validation fails
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true
                };
            });

            // In production, the Angular files will be served from this directory
            // FILES WILL BE CREATED WITH THE 'PublishRunWebpack' BUILD STEP IN 
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
            });
            
            // Pozwala na pobieranie rzeczy z wwwroot
            app.UseSpaStaticFiles();

            // Add the AuthenticationMiddleware to the pipeline 
            app.UseAuthentication();

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
                
                //HomeController - nieobecny?
            });


            //TODO: jak się używa spaservices?
            app.UseSpa(spa =>
            {
                // https://github.com/aspnet/JavaScriptServices
                // https://github.com/aspnet/JavaScriptServices/blob/master/src/Microsoft.AspNetCore.SpaServices.Extensions/SpaApplicationBuilderExtensions.cs                 
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";                

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
            
            // Create a service scope to get an ApplicationDbContext instance using DI
            // TODO: co to jest service scope?
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                // Create the Db if it doesn't exist and applies any pending migration

                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                                                
                dbContext.Database.Migrate();
                DbSeeder.Seed(dbContext, roleManager, userManager);
            }
        }
    }
}