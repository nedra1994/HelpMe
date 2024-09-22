using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using HelpMe.Commun.Security.Identity.Abstraction;
using HelpMe.Commun.Security.Identity.Data;

namespace HelpMe.Commun.Security.Identity.Service
{

    public class AccountManagerService<Tuser,Trole> : IAccountManagerService<Tuser,Trole>
        where Tuser :ApplicationUser
        where Trole : ApplicationRole
    {

        private IdentitySettings _identitySettings;
       
        private IUserRepository<Tuser,Trole> _userRepository;
  
        private ITokenService<Tuser> _tokenService;
        public AccountManagerService(
            IOptions<IdentitySettings> appSettings,
            IUserRepository<Tuser,Trole> userRepository,
            ITokenService<Tuser> tokenservice
           )
        {
            _identitySettings = appSettings.Value;
            _tokenService = tokenservice;
            _userRepository = userRepository;
        }
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public async Task<Tuser> Authenticate(string username, string password)
        {
            var currentUser = await _userRepository.FindByName(username);
            if (currentUser == null) throw new HelpMeAuthenticationException("Login ou mot de passe invalide");
            var signeResult = await _userRepository.CheckPasswordSignIn(currentUser, password, true);
            if (signeResult.Succeeded)
            {
                //if (!currentUser.EmailConfirmed) throw new HelpMeAuthenticationException("Votre email n'est pas encore confirmé");
                if (currentUser.IsDisabled == true) throw new HelpMeAuthenticationException("Votre compte est déactive, Merci de contacter votre administrateur");
                if (currentUser.ExpirationDate <= DateTime.Now) throw new HelpMeAuthenticationException("Votre mot de passe est expiré");
                return currentUser;
            }
           if (signeResult.Value?.IsLockedOut==true) throw new HelpMeAuthenticationException("Votre compte est temporairement désactivé Merci de réessayer plus tard");
           throw new HelpMeAuthenticationException("Login ou mot de passe invalide");

            //// authentication successful so generate jwt token
            //var claim = new Claim[] { new Claim(ClaimTypes.Name, currentUser.UserName), new Claim(ClaimTypes.NameIdentifier, currentUser.UserName) };
            //var token = _tokenService.GenerateAccessToken(claim);
            //var newRefreshToken = _tokenService.GenerateRefreshToken();
            //currentUser.AddRefreshToken(newRefreshToken, remoteIpAddress, _identitySettings.RefreshTokenExpirationTimeMinutes);
            //await _userRepository.Update(currentUser);
            //return new HelpMeIdentityToken()
            //{
            //    SubjectId = currentUser.Id,
            //    UserName = currentUser.UserName,
            //    //token = token,
            //    //refreshToken = newRefreshToken
            //};
        }

        public async Task<Tuser> UpdatePartenaireUser(string userLogin, string newPartenaireId)
        {
            if (string.IsNullOrEmpty(userLogin)) throw new AccountComfirmeException("Username obligatoire");
            if (string.IsNullOrEmpty(newPartenaireId)) throw new AccountComfirmeException("PartenaireId obligatoire");
            
            var currentUser = await _userRepository.FindByName(userLogin);
            if (currentUser == null) throw new AccountComfirmeException("Username invalide");

            currentUser.PartenaireId = newPartenaireId;
            currentUser = _userRepository.Update(currentUser);
            await _userRepository.UnitOfWork.SaveEntitiesAsync();

            return currentUser;
        }

        public async Task<Tuser> CreateAccount(Tuser currentUser, string pw)
        {
            if (currentUser==null) throw new HelpMeCreateAccountException("User invalide");
            if (!IsValidEmail(currentUser.UserName)) throw new HelpMeCreateAccountException("User Name Format invalide");
            currentUser.IsDisabled = false;
            currentUser.ExpirationDate = DateTime.Now.AddDays(_identitySettings.PasswordExpirationTimeDays);

            var createResult = await _userRepository.Create(currentUser, pw);
            if (createResult.Succeeded)
            {
                return await _userRepository.FindByName(currentUser.UserName);
            }
            string Erreur = "";
            foreach (var error in createResult.Errors)
            {
                Erreur += "-" + error.Description + Environment.NewLine;
            }
            throw new HelpMeCreateAccountException(Erreur);
        }
        public async Task<Tuser> ConfirmeCreateAccountRequest(string userName)
        {
            if (string.IsNullOrEmpty(userName))  throw new AccountComfirmeException("user Name obligatoire");
            var currentUser = await _userRepository.FindByName(userName);
            if (currentUser == null) throw new AccountComfirmeException("user Name invalide");
            //if (currentUser.EmailConfirmed) throw new AccountComfirmeException("Account déja Confirmée");
            string confirmationToken = await _tokenService.GenerateEmailConfirmationToken(currentUser);
            currentUser.IsAccountRequestConfirmed = true;
            currentUser.AccountRequestConfirmedDate = DateTime.Now;
            currentUser.AccountRequestConfirmedToken = confirmationToken;
            _userRepository.Update(currentUser);
            return currentUser;
        }
        public async Task<Tuser> ConfirmeEmailAccountRequest(string username, string token)
        {
            if (string.IsNullOrEmpty(username)) throw new AccountComfirmeMailException("user Name obligatoire");
            if (string.IsNullOrEmpty(token)) throw new AccountComfirmeMailException("token obligatoire");
            var currentUser =  await _userRepository.FindByName(username);
            if (currentUser == null) throw new AccountComfirmeMailException("user Name invalide");
          //  if (currentUser.EmailConfirmed) throw new AccountComfirmeMailException("Email déja Confirmée");
            var confirmEmaiResult = await _userRepository.ConfirmEmail(currentUser, token);
            if (confirmEmaiResult.Succeeded) return currentUser;
            string Erreur = "";
            foreach (var error in confirmEmaiResult.Errors)
            {
                Erreur += "-" + error.Description + Environment.NewLine;
            }
            throw new AccountComfirmeMailException(Erreur);
        }
        public async Task<Tuser> ResetPasswordRequest(string username)
        {
            if (string.IsNullOrEmpty(username)) throw new ResetPasswordRequestException("user Name obligatoire");
            var currentUser = await _userRepository.FindByName(username);
            if (currentUser == null) throw new ResetPasswordRequestException("user Name invalide");
            //  if (currentUser.EmailConfirmed) throw new AccountComfirmeMailException("Email déja Confirmée");
            var token = await _tokenService.GeneratePasswordResetToken(currentUser);
            currentUser.ResetPasswordRequestToken = token;
            currentUser.ResetPasswordRequestDate = DateTime.Now;
             _userRepository.Update(currentUser);
            return currentUser;
          
        }
        public async Task<Tuser> ResetPassword(string username, string token,string newPw)
        {
            if (string.IsNullOrEmpty(username)) throw new ResetPasswordException("user Name obligatoire");
            if (string.IsNullOrEmpty(token)) throw new ResetPasswordException("token obligatoire");
            if (string.IsNullOrEmpty(newPw)) throw new ResetPasswordException("Mot de passe obligatoire");
            var currentUser = await _userRepository.FindByName(username);
            if (currentUser == null) throw new ResetPasswordException("user Name invalide");
            //  if (currentUser.EmailConfirmed) throw new AccountComfirmeMailException("Email déja Confirmée");
            var resetPasswordResult = await _userRepository.ResetPassword(currentUser, token, newPw);
            if (resetPasswordResult.Succeeded)
            {
                currentUser.IsDisabled = false;
                if (currentUser.Function == "ExtranetClient")
                {
                    currentUser.ExpirationDate = DateTime.Now.AddDays(_identitySettings.ExtranetPasswordExpirationTimeDays);
                }
                else
                {
                    currentUser.ExpirationDate = DateTime.Now.AddDays(_identitySettings.PasswordExpirationTimeDays);
                }
                await _userRepository.UpdateUser(currentUser);                
                return currentUser;
            }
            string Erreur = "";
            foreach (var error in resetPasswordResult.Errors)
            {
                Erreur += "-" + error.Description + Environment.NewLine;
            }
            throw new ResetPasswordException(Erreur);
        }
        public async Task<Tuser> ChangePassword(string username, string oldPw, string newPw)
        {
            if (string.IsNullOrEmpty(username)) throw new ChangePasswordException("user Name obligatoire");
            if (string.IsNullOrEmpty(oldPw)) throw new ChangePasswordException("Ancien Mot de passe  obligatoire");
            if (string.IsNullOrEmpty(newPw)) throw new ChangePasswordException("Nouveaux Mot de passe obligatoire");

            var currentUser = await _userRepository.FindByName(username);
            if (currentUser == null) throw new ChangePasswordException("user Name invalide");
            //  if (currentUser.EmailConfirmed) throw new AccountComfirmeMailException("Email déja Confirmée");
            var resetPasswordResult = await _userRepository.ChangePassword(currentUser, oldPw, newPw);
            if (resetPasswordResult.Succeeded) return currentUser;
            string Erreur = "";
            foreach (var error in resetPasswordResult.Errors)
            {
                Erreur += "-" + error.Description + Environment.NewLine;
            }
            throw new ChangePasswordException(Erreur);
        }

        //public async Task<Tuser> GetAuthentificationKeyRequest(string username)
        //{
        //    if (string.IsNullOrEmpty(username)) throw new ResetPasswordRequestException("user Name obligatoire");
        //    var currentUser = await _userRepository.FindByName(username);
        //    if (currentUser == null) throw new ResetPasswordRequestException("user Name invalide");
        //    //if (currentUser.EmailConfirmed) throw new AccountComfirmeMailException("Email déja Confirmée");
        //    var token = await _userRepository.GeneratePasswordResetToken(currentUser);
        //    currentUser.ResetPasswordRequestToken = token;
        //    currentUser.ResetPasswordRequestDate = DateTime.Now;
        //    _userRepository.Update(currentUser);
        //    return currentUser;

        //}

        //public async Task<HelpMeIdentityToken> RefreshToken(string token, string refreshToken, string remoteIpAddress)
        //{
        //    try
        //    {
        //        var principal = _tokenService.GetPrincipalFromExpiredToken(token);
        //        var username = principal.Identity.Name; //this is mapped to the Name claim by default

        //        // var currentUser =  await _userRepository.FindByName(username);
        //        var currentUser = await _userRepository.FindByNameWithRefreshToken(username);//..GetSingleBySpec(new UserSpecification<Tuser>(username));
        //        if (currentUser == null || !currentUser.HasValidRefreshToken(refreshToken)) throw new HelpMeRefreshTokenException("Invalide Refresh Token");

        //        var newJwtToken = _tokenService.GenerateAccessToken(principal.Claims);
        //        var newRefreshToken = _tokenService.GenerateRefreshToken();

        //        currentUser.RemoveRefreshToken(refreshToken);
        //        currentUser.AddRefreshToken(newRefreshToken, remoteIpAddress, _identitySettings.RefreshTokenExpirationTimeMinutes);

        //         await _userRepository.Update(currentUser);

        //        return new HelpMeIdentityToken()
        //        {

        //            UserName = currentUser.UserName,
        //            token = newJwtToken,
        //            refreshToken = newRefreshToken
        //        };
        //    }
        //    catch (SecurityTokenException ex )
        //    {

        //        throw new HelpMeRefreshTokenException(ex.Message);
        //    }

        //}
        //public async Task RevokeRefreshToken(string username, string refreshToken)
        //{
        //    //HelpMeRevokeTokenException
        //    var currentUser =  await _userRepository.FindByName(username);
        //    if (currentUser == null)  throw new HelpMeRevokeTokenException("Invalide username");
        //    currentUser.RemoveRefreshToken(refreshToken);
        //    await _userRepository.Update(currentUser);
        //}
    }
}
