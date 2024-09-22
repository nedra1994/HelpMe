

using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.Services.Identity.API.Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using HelpMe.Commun.Security.Identity.Abstraction;
using HelpMe.Commun.Security.Identity.Data;
using HelpMe.Commun.Security.Identity.Service;
using HelpMe.Commun.Security.Identity.Service.IdentityServer4;
using HelpMe.Commun.Security.Identity.TokenProvider;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
//using Web.Api.Infrastructure.Data.Repositories;

namespace HelpMe.Commun.Security.Identity.Extension
{
    public static class MicrosoftIdentityExtension
    {

        public static Microsoft.Extensions.DependencyInjection.IServiceCollection  AddHelpMeIdentityServer<Tuser,Trole, Tcontext>  (
            this Microsoft.Extensions.DependencyInjection.IServiceCollection services 
            , IConfiguration configuration)
            where Tuser : ApplicationUser
            where Trole : ApplicationRole
            where Tcontext : ApplicationDbContext<Tuser,Trole>
        {

            
            var identitySettingsSection = configuration.GetSection("IdentitySettings");
            var appSettings = identitySettingsSection.Get<IdentitySettings>();

            services.AddDbContext<Tcontext>(options => options.UseSqlServer(appSettings.ConnectionString));
            services.Configure<IdentitySettings>(identitySettingsSection);
           
            services.AddIdentity<Tuser, Trole>(
               (option) =>
               {
                   option.Password.RequiredLength = appSettings.PasswordRequiredLength;
                   option.Password.RequireDigit = appSettings.PasswordIsRequireDigit;
                   option.Password.RequireLowercase = appSettings.PasswordIsRequireLowercase;
                   option.Password.RequireNonAlphanumeric = appSettings.PasswordIsRequireNonAlphanumeric;
                   option.Password.RequireUppercase = appSettings.PasswordIsRequireUppercase;
                   option.Lockout.MaxFailedAccessAttempts = appSettings.PasswordMaxFailedAccessAttempts;
                   option.Tokens.EmailConfirmationTokenProvider = "HelpMeEmailConfirmationTokenProvider";
                   option.Tokens.PasswordResetTokenProvider = "HelpMePasswordResetTokenProvider";
                   option.User.RequireUniqueEmail = true;

               }
               )
                .AddEntityFrameworkStores<Tcontext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<HelpMeEmailConfirmationTokenProvider<Tuser>>("HelpMeEmailConfirmationTokenProvider")
                .AddTokenProvider<HelpMePasswordResetTokenProvider<Tuser>>("HelpMePasswordResetTokenProvider");

            services.Configure<HelpMePasswordResetTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(appSettings.PasswordResetTokenExpirationTimeHours));
            services.Configure<HelpMeEmailConfirmationTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(appSettings.EmailConfirmationTokenExpirationTimeHours));

            // prevent from mapping "sub" claim to nameidentifier.

          

          
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            //}).AddJwtBearer(options =>
            //{
            //    options.Authority = appSettings.IdentityUrl;
            //    options.RequireHttpsMetadata = false;
            //    options.Audience = "account";
            //});

            // configure jwt authentication

            //var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);
            //services.AddAuthentication(x =>
            //{
            //    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //        .AddJwtBearer(x =>
            //{
            //    x.RequireHttpsMetadata = false;
            //    x.SaveToken = true;
            //    x.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidIssuer = appSettings.Issuer,
            //        ValidAudience = appSettings.Audience,
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(key),
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ClockSkew = TimeSpan.Zero
            //    };
            //    x.Events = new JwtBearerEvents
            //    {
            //        OnAuthenticationFailed = context =>
            //        {
            //            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            //            {
            //                context.Response.Headers.Add("Token-Expired", "true");
            //            }
            //            return Task.CompletedTask;
            //        }
            //    };
            //});




            var migrationsAssembly = typeof(Tuser).GetTypeInfo().Assembly.GetName().Name;
            services.AddIdentityServer(x =>
            { 
                x.IssuerUri = "null";
               x.KeyManagement.Enabled=true;
                x.EmitStaticAudienceClaim = true;
               // x.TokenValidationParameters.ValidateAudience = false;
                x.Authentication.CookieLifetime = TimeSpan.FromHours(2);
            })
            // .AddDevspacesIfNeeded(Configuration.GetValue("EnableDevspaces", false))
          //  .AddSigningCredential(Certificate.Get(configuration["SigningCredentialCert"]))
            .AddAspNetIdentity<Tuser>()
            .AddResourceOwnerValidator<AccountPasswordValidatorService<Tuser,Trole>>()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(appSettings.ConnectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(appSettings.ConnectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            })
            .Services.AddTransient<IProfileService, ProfileService<Tuser,Trole>>();

         //   services.AddScoped(typeof(IUserRepository<Tuser,Trole>), typeof(UserRepository<Tuser,Trole>));
            services.AddScoped(typeof(IAccountManagerService<Tuser,Trole>), typeof(AccountManagerService<Tuser,Trole>));
            services.AddScoped(typeof(ITokenService<>), typeof(TokenService<>));
            services.AddTransient(typeof(ApplicationDbContext<Tuser,Trole>), typeof(Tcontext));
            
            return services;
        }
    }
}
