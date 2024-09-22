using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Netcom.Commun.Infra.Data.EF.Repositories;
using Netcom.Commun.Security.Identity.Abstraction;
using Netcom.Commun.Security.Identity.Data;
using System.Collections.Generic;
using Netcom.Commun.Security.Identity.Specifications;

namespace Web.Api.Infrastructure.Data.Repositories
{


    internal sealed class UserRepository<Tuser,Trole> : EfRepository<Tuser>, IUserRepository<Tuser,Trole>
        where Tuser : ApplicationUser
        where Trole : ApplicationRole
    {
     


        private readonly UserManager<Tuser> _userManager;
        private readonly SignInManager<Tuser> _signInManager;
       
        public UserRepository(
            ApplicationDbContext<Tuser,Trole> appDbContext,
            UserManager<Tuser> userManager,
            SignInManager<Tuser> signInManager
            ):base(appDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            

        }
        public async Task<Result<Tuser>> ChangePassword(Tuser currentUser, string oldPw, string newPw)
        {
            var result = await _userManager.ChangePasswordAsync(currentUser, oldPw, newPw);
            return new Result<Tuser>(result.Succeeded, ToErreurList(result.Errors));
        }


        public async Task<Result<LogInResult>> CheckPasswordSignIn(Tuser currentUser, string password, bool lockedOut)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(currentUser, password, lockedOut);
            return new Result<LogInResult>(result.Succeeded, null, new LogInResult(result.IsLockedOut, result.IsNotAllowed));
        }

        public async Task<Result<Tuser>> ConfirmEmail(Tuser currentUser, string token)
        {
            var result = await _userManager.ConfirmEmailAsync(currentUser, token);
            return new Result<Tuser>(result.Succeeded, ToErreurList(result.Errors));
        }

        public async Task<Result<Tuser>> Create(Tuser currentUser, string pw)
        {
            var result = await _userManager.CreateAsync(currentUser, pw);
            return new Result<Tuser>(result.Succeeded, ToErreurList(result.Errors));
        }

        public async Task<Tuser> FindByName(string username)
        {
            var result = await _userManager.FindByNameAsync(username);
            return result;
        }

        //public async Task<Tuser> FindByNameWithRefreshToken(string username)
        //{
        //    var result = await GetSingleBySpec(new UserSpecification<Tuser>(username));
        //    return result;
        //}

        public async Task<Result<Tuser>> ResetPassword(Tuser currentUser, string token, string newPw)
        {
            var result = await _userManager.ResetPasswordAsync(currentUser, token, newPw);
            return new Result<Tuser>(result.Succeeded, ToErreurList(result.Errors));
        }

        private IEnumerable<Erreur> ToErreurList(IEnumerable<IdentityError> errors)
        {
            List<Erreur> _errors = new List<Erreur>();
            foreach (var item in errors)
            {
                _errors.Add(new Erreur(item.Code, item.Description));
            }
            return _errors;
        }


        public async Task<Result<Tuser>> UpdateUser(Tuser currentUser)
        {
            var result = await _userManager.UpdateAsync(currentUser);
            return new Result<Tuser>(result.Succeeded, ToErreurList(result.Errors));
        }

        //public async Task<string> GenerateEmailConfirmationToken(Tuser currentUser)
        //{
        //    var result = await _userManager.GenerateEmailConfirmationTokenAsync(currentUser);
        //    return result;
        //}

        //public async Task<IdentityResult> SetAuthenticationToken(Tuser currentUser,string provider, string tokenName, string token)
        //{
        //    var result = await _userManager.SetAuthenticationTokenAsync(currentUser, provider, tokenName, token);
        //    return result;
        //}

        //public async Task<string> GetAuthenticationToken(Tuser currentUser, string provider, string tokenName)
        //{
        //    var result = await _userManager.GetAuthenticationTokenAsync(currentUser,provider,tokenName);
        //    return result;
        //}

        //public async Task<System.Security.Claims.ClaimsPrincipal> RegenerateAuthenticationToken(Tuser currentUser, string provider, string tokenName)
        //{
        //    var result = await _signInManager.CreateUserPrincipalAsync(currentUser);//, provider, tokenName);
        //    return result;
        //}
    }

  
}
