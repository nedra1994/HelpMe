using IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using HelpMe.Commun.Security.Identity.Abstraction;
using HelpMe.Commun.Security.Identity.Data;
using System.Threading.Tasks;

namespace HelpMe.Commun.Security.Identity.Service.IdentityServer4
{
    public class AccountPasswordValidatorService<Tuser,Trole> : IResourceOwnerPasswordValidator
         where Tuser : ApplicationUser
        where Trole:ApplicationRole
    {
        private readonly IAccountManagerService<Tuser,Trole> _accountManager;
        ///private readonly IUserRepository<Tuser> _userRepository;
        public AccountPasswordValidatorService(IAccountManagerService<Tuser,Trole> accountManager)// , IUserRepository<Tuser>  userRepository)
        {
           // _userRepository = userRepository;
            _accountManager= accountManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                var user = await _accountManager.Authenticate(context.UserName, context.Password);
                context.Result = new GrantValidationResult(user.Id, OidcConstants.AuthenticationMethods.Password);
            }
            catch (System.Exception ex)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, ex.Message);
            }

        }
    }
}