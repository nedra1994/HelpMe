using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace HelpMe.Commun.Security.Identity.IdentityServer4.NewFolder
{


    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
        public int MinimumAge { get; }

        public MinimumAgeRequirement(int minimumAge)
        {
            MinimumAge = minimumAge;
        }
    }


    public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       MinimumAgeRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth &&
                                            c.Issuer == "http://contoso.com"))
            {
                //TODO: Use the following if targeting a version of
                //.NET Framework older than 4.6:
                //      return Task.FromResult(0);
                return Task.CompletedTask;
            }

            var dateOfBirth = Convert.ToDateTime(
                context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth &&
                                            c.Issuer == "http://contoso.com").Value);

            int calculatedAge = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth > DateTime.Today.AddYears(-calculatedAge))
            {
                calculatedAge--;
            }

            if (calculatedAge >= requirement.MinimumAge)
            {
                context.Succeed(requirement);
            }

            //TODO: Use the following if targeting a version of
            //.NET Framework older than 4.6:
            //      return Task.FromResult(0);
            return Task.CompletedTask;
        }
    }


    // This attribute derives from the [Authorize] attribute, adding 
    // the ability for a user to specify an 'age' paratmer. Since authorization
    // policies are looked up from the policy provider only by string, this
    // authorization attribute creates is policy name based on a constant prefix
    // and the user-supplied age parameter. A custom authorization policy provider
    // (`MinimumAgePolicyProvider`) can then produce an authorization policy with 
    // the necessary requirements based on this policy name.
    internal class HelpMeRequireClaimAttribute : AuthorizeAttribute
    {
        const string POLICY_PREFIX = "HelpMeRequireClaim";

        public HelpMeRequireClaimAttribute(string claim) =>  _claim= claim;

        // Get or set the Age property by manipulating the underlying Policy property
        private string _claim
        {
           
            set
            {
                Policy = $"{POLICY_PREFIX}{value.ToString()}";
            }
        }
    }
    internal class RequireClaimPolicyProvider : IAuthorizationPolicyProvider
    {
        const string POLICY_PREFIX = "HelpMeRequireClaim";
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public RequireClaimPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            // ASP.NET Core only uses one authorization policy provider, so if the custom implementation
            // doesn't handle all policies (including default policies, etc.) it should fall back to an
            // alternate provider.
            //
            // In this sample, a default authorization policy provider (constructed with options from the 
            // dependency injection container) is used if this custom provider isn't able to handle a given
            // policy name.
            //
            // If a custom policy provider is able to handle all expected policy names then, of course, this
            // fallback pattern is unnecessary.
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        // Policies are looked up by string name, so expect 'parameters' (like age)
        // to be embedded in the policy names. This is abstracted away from developers
        // by the more strongly-typed attributes derived from AuthorizeAttribute
        // (like [MinimumAgeAuthorize] in this sample)
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                var claimType = policyName.Substring(POLICY_PREFIX.Length);
                if (!string.IsNullOrEmpty(claimType))
                {
                    var policy = new AuthorizationPolicyBuilder();
                    policy.RequireClaim(claimType);
                    return Task.FromResult(policy.Build());
                }
            }

            // If the policy name doesn't match the format expected by this policy provider,
            // try the fallback provider. If no fallback provider is used, this would return 
            // Task.FromResult<AuthorizationPolicy>(null) instead.
            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
           return  FallbackPolicyProvider.GetFallbackPolicyAsync();
        }
    }

}

