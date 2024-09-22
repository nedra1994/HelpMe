using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using HelpMe.Commun.Security.Identity.ClientAuth.Authorization;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace HelpMe.Commun.Security.Identity.ClientAuth
{
    public static class HelpMeClientAuthExtention
    {
        

        public static IServiceCollection AddHelpMeClientAuth(this IServiceCollection services , IConfiguration configuration, string audience)
           
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
            var identitySettingsSection = configuration.GetSection("IdentitySettings");
            var IdentityUrl = identitySettingsSection.GetValue<string>("IdentityUrl");
            services.AddSingleton<IAuthorizationPolicyProvider, RequireClaimPolicyProvider>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = IdentityUrl;
                options.RequireHttpsMetadata = false;
                options.Audience = audience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
            });
            return services;
        }
    }
}
