using Duende.IdentityServer.Services;
using IdentityModel;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using HelpMe.Commun.Security.Identity.Abstraction;
using HelpMe.Commun.Security.Identity.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HelpMe.Commun.Security.Identity.Service.IdentityServer4
{
    public class ProfileService<Tuser,Trole> : IProfileService
         where Tuser : ApplicationUser
        where Trole: ApplicationRole
    {
        private readonly UserManager<Tuser> _userManager;
        private readonly RoleManager<Trole> _roleManager;
        private readonly IUserRepository<Tuser,Trole> _userRepository;

        IdentitySettings _appSettings;
        public ProfileService(UserManager<Tuser> userManager , 
            RoleManager<Trole>  roleManager,   
            IUserRepository<Tuser,Trole> userRepository,
            IOptions<IdentitySettings> appSettings
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepository = userRepository;
            _appSettings = appSettings.Value;
        }

        async public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value;

            
            var user = await _userManager.FindByIdAsync(subjectId);
            if (user == null)
                throw new ArgumentException("Invalid subject identifier");

            var partenaire = user.GetType().GetProperty("Partenaire").GetValue(user, null);

            var partnersFils = await _userManager.FindByIdAsync(subjectId);

            string partenaireCode = "";
            string partenaireNom = "";
            string partenaireCodeOperateur = "";
            string partenaireLogoPath = "";

            bool IsHelpMe = false;
            bool HasProduitAntivirus = false;
            bool HasTrancheMsisdnOrange = false;
            bool HasTrancheMsisdnSFR = false;
            bool HasTrancheMsisdnBTBD = false;
            bool HasAnnexeFacture = false;
            bool HideFacturation = false;
            bool HideGsmProfil = false;
            bool IsNotPrelevement = false;
            bool IsLimitAcces = false;
            string PartenaireCdrModele = "";
            bool ShowDetailFacture = false;
            var TypePartner = "Partner";
            bool isParticipating = false;

            if (partenaire!= null)
            {
                partenaireCode = partenaire.GetType().GetProperty("Code").GetValue(partenaire, null).ToString();
                partenaireNom = partenaire.GetType().GetProperty("RaisonSociale").GetValue(partenaire, null).ToString();
                partenaireCodeOperateur = partenaire.GetType().GetProperty("CodeOperateur").GetValue(partenaire, null).ToString(); 

                var result = GetValueFromObject(partenaire, "LogoPath");
                if (result != null) partenaireLogoPath = result.ToString();

                result = GetValueFromObject(partenaire, "IsHelpMe");
                if (result != null) IsHelpMe = bool.Parse(result.ToString());

                result = GetValueFromObject(partenaire, "HideFacturation");
                if (result != null) HideFacturation = bool.Parse(result.ToString());

                result = GetValueFromObject(partenaire, "HideGsmProfil");
                if (result != null) HideGsmProfil = bool.Parse(result.ToString());

                result = GetValueFromObject(partenaire, "IsNotPrelevement");
                if (result != null) IsNotPrelevement = bool.Parse(result.ToString());

                result = GetValueFromObject(partenaire, "HasProduitAntivirus");
                if (result != null) HasProduitAntivirus = bool.Parse(result.ToString());

                result = GetValueFromObject(partenaire, "HasTrancheMsisdnOrange");
                if (result != null) HasTrancheMsisdnOrange = bool.Parse(result.ToString());

                result = GetValueFromObject(partenaire, "TypePartner");
                if (result != null) TypePartner = result.ToString();

                result = GetValueFromObject(partenaire, "HasTrancheMsisdnSFR");
                if (result != null) HasTrancheMsisdnSFR = bool.Parse(result.ToString());

                result = GetValueFromObject(partenaire, "HasTrancheMsisdnBTBD");
                if (result != null) HasTrancheMsisdnBTBD = bool.Parse(result.ToString());


                result = GetValueFromObject(partenaire, "CodeModelCdr");
                if (result != null) PartenaireCdrModele = result.ToString();

                result = GetValueFromObject(partenaire, "ShowDetailFacture");
                if (result != null) ShowDetailFacture = bool.Parse(result.ToString());


                result = GetValueFromObject(partenaire, "IsLimitAcces");
                if (result != null) IsLimitAcces = bool.Parse(result.ToString());

                result = GetValueFromObject(partenaire, "isParticipating");
                if (result != null) isParticipating = bool.Parse(result.ToString());

            }
            
            var claims = await GetClaimsFromUser(user, partenaireCode, partenaireNom, partenaireCodeOperateur
                , IsHelpMe, HideFacturation, HideGsmProfil, IsNotPrelevement, HasProduitAntivirus, HasTrancheMsisdnOrange,HasTrancheMsisdnSFR,HasTrancheMsisdnBTBD, HasAnnexeFacture, partenaireLogoPath, PartenaireCdrModele, ShowDetailFacture, IsLimitAcces, isParticipating);
            context.IssuedClaims = claims.ToList();
        }


        private object GetValueFromObject (object partenaire, string propertyName)
        {
            try
            {
                var result = partenaire.GetType().GetProperty(propertyName).GetValue(partenaire, null);
                return result;
            }
            catch
            {
                return null;
            }
        }

            async public Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(subjectId);

            context.IsActive = false;

            if (user != null)
            {
                if (_userManager.SupportsUserSecurityStamp)
                {
                    var security_stamp = subject.Claims.Where(c => c.Type == "security_stamp").Select(c => c.Value).SingleOrDefault();
                    if (security_stamp != null)
                    {
                        var db_security_stamp = await _userManager.GetSecurityStampAsync(user);
                        if (db_security_stamp != security_stamp)
                            return;
                    }
                }

                context.IsActive = user.IsDisabled == false && (!user.LockoutEnabled || !user.LockoutEnd.HasValue ||user.LockoutEnd <= DateTime.Now);
            }
        }

        private async Task<IEnumerable<Claim>> GetClaimsFromUser(Tuser user, string partenaireCode, string partenaireNom, string PartenaireCodeOperateur
                                        , bool IsHelpMe, bool HideFacturation, bool HideGsmProfil, bool IsNotPrelevement, bool HasProduitAntivirus, bool HasTrancheMsisdnOrange, bool HasTrancheMsisdnSFR, bool HasTrancheMsisdnBTBD, bool HasAnnexeFacture, string partenaireLogoPath, string PartenaireCdrModele, bool ShowDetailFacture, bool isLimitAcces, bool isParticipating)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            if (!string.IsNullOrWhiteSpace(user.Function)) claims.Add(new Claim("function", user.Function));
            if (!string.IsNullOrWhiteSpace(user.PartnersAccessType)) claims.Add(new Claim("partnersAccessType", user.PartnersAccessType));
            if (!string.IsNullOrWhiteSpace(user.Name)) claims.Add(new Claim("username", user.Name));
            if (!string.IsNullOrWhiteSpace(user.Email)) claims.Add(new Claim("userMail", user.Email));
            if (!string.IsNullOrWhiteSpace(user.PhoneNumber)) claims.Add(new Claim("userTel", user.PhoneNumber));
            if (!string.IsNullOrWhiteSpace(user.PartenaireId)) claims.Add(new Claim("PartenaireId", user.PartenaireId));
            if (!string.IsNullOrWhiteSpace(user.ClientCode)) claims.Add(new Claim("ClientCode", user.ClientCode));
            if (!string.IsNullOrWhiteSpace(user.PartnersAccessType)) claims.Add(new Claim("PartnersAccessType", user.PartnersAccessType));


            if (!string.IsNullOrWhiteSpace(partenaireCode)) claims.Add(new Claim("PartenaireCode", partenaireCode));
            if (!string.IsNullOrWhiteSpace(partenaireNom)) claims.Add(new Claim("PartenaireNom", partenaireNom));
            if (!string.IsNullOrWhiteSpace(PartenaireCodeOperateur)) claims.Add(new Claim("PartenaireCodeOperateur", PartenaireCodeOperateur));


            try
            {

                if (!string.IsNullOrWhiteSpace(partenaireLogoPath))
                {
                    if (!Directory.Exists(_appSettings.ApplicationImagePath))
                        Directory.CreateDirectory(_appSettings.ApplicationImagePath);
                    //copy the logo to image folder
                    File.Copy(partenaireLogoPath, Path.Combine(_appSettings.ApplicationImagePath, Path.GetFileName(partenaireLogoPath)), true);
                    claims.Add(new Claim("partenaireLogoPath", Path.GetFileName(partenaireLogoPath)));
                }
            }
            catch
            {

            }

            claims.Add(new Claim("IsHelpMe", IsHelpMe.ToString()));
            claims.Add(new Claim("HideFacturation", HideFacturation.ToString()));
            claims.Add(new Claim("HideGsmProfil", HideGsmProfil.ToString()));
            claims.Add(new Claim("IsNotPrelevement", IsNotPrelevement.ToString()));
            claims.Add(new Claim("HasProduitAntivirus", HasProduitAntivirus.ToString()));
            claims.Add(new Claim("HasTrancheMsisdnOrange", HasTrancheMsisdnOrange.ToString()));
            claims.Add(new Claim("HasTrancheMsisdnSFR", HasTrancheMsisdnSFR.ToString()));
            claims.Add(new Claim("HasTrancheMsisdnBTBD", HasTrancheMsisdnBTBD.ToString()));

            if (isLimitAcces)
                claims.Add(new Claim("isLimitAcces", isLimitAcces.ToString()));
            claims.Add(new Claim("isParticipating", isParticipating.ToString()));
            claims.Add(new Claim("PartenaireCdrModele", PartenaireCdrModele));
            claims.Add(new Claim("ShowDetailFacture", ShowDetailFacture.ToString()));

            //Users Claims
            var lstUserClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(lstUserClaims.Select(userClaim => new Claim(userClaim.Type, userClaim.Value)));
            claims.Add(new Claim("ConnectUser", user.UserName));
            claims.Add(new Claim("isDoubleAuth", user.isDoubleAuth.ToString()));
            claims.Add(new Claim("DateCreateCode", user.DateCreateCode.ToString()));
            if (user.CodeConfirmation != null)  claims.Add(new Claim("CodeConfirmation", user.CodeConfirmation.ToString()));
            var DateExp = DateTime.Now.AddHours(3);
            claims.Add(new Claim("DateExpire", DateExp.Year.ToString() + "-" + DateExp.Month.ToString("00") + "-" + DateExp.Day.ToString("00") + " " + DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00")));
            //
            //"ConnectUser": "testworking",
            //"DateExpire": "2020-03-09 11:29"

            //Roles Claims
            var lstRoles = await _userManager.GetRolesAsync(user);
            foreach (var roleName in lstRoles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                var lstRoleClaims = await _roleManager.GetClaimsAsync(role);
                //claims.AddRange(lstRoleClaims.Select(roleClaim => new Claim(roleClaim.Type, roleClaim.Value)));
                claims.AddRange(lstRoleClaims.Select(roleClaim => new Claim(roleClaim.Type, "1")));
            }

            if (user.Function == "Admin" || user.Function == "UserAdmin")
            {
                if (isLimitAcces)
                {
                    foreach (var rfunc in await _userRepository.GetPartenaireFonctinalites(user))
                    {
                        //claims.Add(new Claim(rfunc, rfunc));
                        claims.Add(new Claim(rfunc, "1"));
                    }
                }
                else
                {
                    var allFonctinalitesNotSU = await _userRepository.GetAllFonctinalitesNotSU();
                    foreach (var rfunc in allFonctinalitesNotSU)
                    {
                        //claims.Add(new Claim(rfunc, rfunc));
                        claims.Add(new Claim(rfunc, "1"));
                        
                    }
                }
            }

            //TODO : Delete Fonction SU si Non Utilisaterur SU / ROOT
            var fonctionsSU  = await _userRepository.GetFonctinalitesSU();

            //--------------------------------------------------------------
            //If user is not Super User (Phenix) ==> Delete Fonctions Super User
            //--------------------------------------------------------------
            if (user.Function != "HelpMeSU"  && user.Function != "HelpMeRoot")
            {
                claims.RemoveAll(y => fonctionsSU.Where(x => x == y.Type).FirstOrDefault() != null);

                //foreach (var claim in claims)
                //{
                //    if (fonctionsSU.Where(x => x == claim.Type).FirstOrDefault() != null)
                //    {
                //        claims.Remove(claim);
                //    }
                //}
            }

            ///WEbSSH
            //if (_appSettings.WebSSHUrl != null)
            //{
            //    claims.Add(new Claim("Wssh", _appSettings.WebSSHUrl));
            //}

            return  claims;

        }
    }
}
