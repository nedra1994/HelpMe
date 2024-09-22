using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace HelpMe.Commun.Security.Identity.ClientAuth.Authorization
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
    public class HelpMeAuthorizeFeaturesAttribute : AuthorizeAttribute
    {
        const string POLICY_PREFIX = "HelpMeRequireClaim";

        public HelpMeAuthorizeFeaturesAttribute(string claim) =>  _claim= claim;

        // Get or set the Age property by manipulating the underlying Policy property
        private string _claim
        {
           
            set
            {
                Policy = $"{POLICY_PREFIX}{value.ToString()}";
            }
        }
    }
}

