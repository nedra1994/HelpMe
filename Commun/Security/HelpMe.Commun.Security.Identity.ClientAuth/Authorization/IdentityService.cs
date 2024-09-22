
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpMe.Commun.Security.Identity
{
    public class 
        IdentityService : IIdentityService
    {
        private IHttpContextAccessor _context; 

        public IdentityService(IHttpContextAccessor context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public string GetUserIdentity()
        {
            try
            {
                return _context.HttpContext.User.FindFirst("sub").Value;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public ClaimsPrincipal User
        {
            get {
                return _context.HttpContext.User;
            }
        }


        public string GetUserRoles()
        {
            try
            {
                return _context.HttpContext.User.FindFirst("sub").Value;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string GetUserMail()
        {
            try
            {
                return _context.HttpContext.User.FindFirst("userMail").Value;
            }
            catch (Exception)
            {

                return "";
            }
        }
        public string GetUserTel()
        {
            try
            {
                return _context.HttpContext.User.FindFirst("userTel").Value;
            }
            catch (Exception)
            {

                return "";
            }
          
        }

        public string GetUserFunction()
        {
            try
            {
                return _context.HttpContext.User.FindFirst("function").Value;

            }
            catch (Exception)
            {
                return "";
            }
        }

        public string GetPartnersAccessType()
        {
            try
            {
                return _context.HttpContext.User.FindFirst("partnersAccessType").Value;
            }
            catch (Exception)
            {
                return "ALL";
            }
        }

        public string GetUserName()
        {

            try
            {
                return _context.HttpContext.User.Identity.Name;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public bool IsAdmin()
        {
            return GetUserFunction() == "Admin";
        }

        

        public string GetPartenaireId()
        {
            try
            {
                return _context.HttpContext.User.FindFirst("PartenaireId").Value;
            }
            catch { return ""; }
        }
        public string isLimitAcces()
        {
            try
            {
                return _context.HttpContext.User.FindFirst("isLimitAcces")?.Value;
            }
            catch { return ""; }
        }

        public bool isFromCRM()
        {
            try
            {
                var synchroContratclaims = _context.HttpContext.User.FindFirst("SynchroContrat")?.Value;
                return  ! string.IsNullOrEmpty(synchroContratclaims);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string GetPartenaireCodeOperteur()
        {
            try
            {
                return _context.HttpContext.User.FindFirst("PartenaireCodeOperateur").Value;
            }
            catch { return ""; }
        }

        public string GetPartenaireCode()
        {
            try
            {
                return _context.HttpContext.User.FindFirst("PartenaireCode").Value;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public string GetPartenaireNom()
        {
            try
            {
                return _context.HttpContext.User.FindFirst("PartenaireNom").Value;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public bool GetPartenaireIsHelpMe()
        {
            try
            {
                return bool.Parse(_context.HttpContext.User.FindFirst("IsHelpMe").Value);
            }
            catch (Exception ex)
            {
                return false ;
            }
        }

        public bool GetPartenaireHideFacturation()
        {
            try
            {
                return bool.Parse(_context.HttpContext.User.FindFirst("HideFacturation").Value);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool GetPartenaireHideGsmProfil()
        {
            try
            {
                return bool.Parse(_context.HttpContext.User.FindFirst("HideGsmProfil").Value);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool GetPartenaireIsNotPrelevement()
        {
            try
            {
                return bool.Parse(_context.HttpContext.User.FindFirst("IsNotPrelevement").Value);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool GetPartenaireHasProduitAntivirus()
        {
            try
            {
                return bool.Parse(_context.HttpContext.User.FindFirst("HasProduitAntivirus").Value);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool GetPartenaireHasTrancheMsisdn()
        {
            try
            {
                return bool.Parse(_context.HttpContext.User.FindFirst("HasTrancheMsisdn").Value);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        
        //public bool HasAcces(string partenaireId, string userLogin)
        //{
        //    if (userLogin == GetUserName()) return true;
        //    if (partenaireId == GetPartenaireId()) return true;


        //    //TODO : Check Fonction si n'est ADMIN
        //    return false ;
        //}

        //public bool HasAcces(string partenaireId)
        //{
        //    if (partenaireId == GetPartenaireId()) return true;

        //    return false;
        //}


        public string GetAllUserIdsPartenaire()
        {
            try
            {
                return _context.HttpContext.User.FindFirst("AllUserIdsPartenaire").Value;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public string GetName()
        {
            try
            {
                return _context.HttpContext.User.FindFirst("username").Value;
            }
            catch (Exception)
            {

                return "";
            }
        }

        public string GetPartenaireCdrModele()
        {
            try
            {
                return _context.HttpContext.User.FindFirst("PartenaireCdrModele").Value;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public bool GetShowDetailFacture()
        {
            try
            {
                return bool.Parse(_context.HttpContext.User.FindFirst("ShowDetailFacture").Value);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CheckAccessRight(string fonction, bool isSuperAdminFunction)
        {
            
            var function = _context.HttpContext.User.FindFirst("function").Value;
            
            var isHelpMeRoot = function == "HelpMeRoot"  ; //All Rights
            var isHelpMeSU = function == "HelpMeSU";
            var isPartnerAdmin = function == "Admin";

            if (isHelpMeRoot) return true;
            if (fonction == "RootFunction" && !isHelpMeRoot) return false ;

            if (isSuperAdminFunction  && !isHelpMeSU) return false;

            //--------------------------------------------------------------------
            if (fonction.StartsWith("SU_") && !isHelpMeSU) return false;
            if (fonction.StartsWith("HelpMeSU_") && !isHelpMeSU) return false;
            if (fonction.StartsWith("fHelpMeSU_") && !isHelpMeSU) return false;
            //--------------------------------------------------------------------

            var hasAccessToFunction = _context.HttpContext.User.FindFirst(fonction);

            //var hasAccess = isPartnerAdmin || isHelpMeRoot || hasAccessToFunction != null;
            var hasAccess = isHelpMeRoot || hasAccessToFunction != null;

            return hasAccess;
        }
    }

    public class Roles
    {
        public string Admin = "Admin";
        public string Manager = "Manager";
        public string Commercial = "Commercial";
    }
}
