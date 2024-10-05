using HelpMe.Commun.Infra.Mailling.Abstraction;
using HelpMe.Commun.Infra.Mailling.Net;
using HelpMe.Commun.Security.Identity.Abstraction;
using HelpMe.Commun.Security.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HelpMe.Web.Identity.Api.Data;
using HelpMe.Commun.Security.Identity.ClientAuth;
using HelpMe.Commun.Infra.Logging.Serilog;
using HelpMe.Commun.Tools.SwaggerUI;
using HelpMe.Commun.Security.Identity.Extension;
using HelpMe.Web.Identity.Api.Models;
using Netcom.Phenix.API.Helpers;

namespace HelpMe.Web.Identity.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            //services.AddTransient<IAccountManagerService>(AccountManagerService);
            // services.AddNetcomIdentityServer<NetcomResellerUser, RolePartenaire, NetcomResellerDBContext>(Configuration);

            // services.AddNetcomHealthChecks()
            //.InitFromConfig(Configuration)
            //services.AddDbContextCheck<IdentityDbContext>("DataBase [IdentityDbContext]", HealthStatus.Unhealthy, new[] { "mb", "infra", "DataBase" })
            services.AddHelpMeIdentityServer<HelpMeResellerUser, RoleUser, IdentityDbContext>(Configuration);


            //.ForwardToPrometheus();

            services.AddHelpMeClientAuth(Configuration, "account");

            var maillingSettingsSection = Configuration.GetSection("MaillingSettings");
            services.AddHelpMeSerilog<HttpContextAccessor>();
            services.Configure<MaillingConfig>(maillingSettingsSection);
            services.AddSingleton<IMaillingService, MaillingService>();
            services.AddTransient<IIdentityService, IdentityService>();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddHelpMeSwagger("HelpMe.Reseller.Identity.Api", "v1");
            //services.AddScoped<IFonctionnaliteRepository, FonctionnaliteRepository>();
            //services.AddScoped<IPartenaireCompanyRepository, PartenaireCompanyRepository>();
            //services.AddScoped<IPartenaireFonctionnaliteRepository, PartenaireFonctionnaliteRepository>();

            //services.AddScoped<ISecuriteService, SecuriteService>();

            //services.AddScoped(typeof(IUserRepository<NetcomResellerUser, RolePartenaire>), typeof(NetcomUserRepository<NetcomResellerUser, RolePartenaire>));
            //services.AddScoped<IExceptionService, ExceptionService>();

            //services.AddScoped<IPartnersAccessRepository, PartnersAccessRepository>();
            //services.AddScoped<IPartnerUniversRepository, PartnerUniversRepository>();

            //services.AddScoped<IAutorisationIpRepository, AutorisationIpRepository>();

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue; // Set to the maximum size you want to allow.

            });
            //services.Configure<IISServerOptions>(options =>
            //{
            //    options.MaxRequestHeaderSize = int.MaxValue; // Set to the maximum size you want to allow.

            //});

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestHeadersTotalSize = int.MaxValue;  // 64 KB
            });



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
            }

            app.UseHelpMeSwagger("HelpMe.Reseller.Identity.Api");

            app.UseHttpsRedirection();
            app.UseRouting();
           // app.UseNetcomHealthChecks();
            app.UseCors("CorsPolicy");
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
               // endpoints.MapNetcomHealthChecks();
            });
        }



    }
}
