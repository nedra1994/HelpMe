using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HelpMe.Commun.Security.Identity
{
    public interface IIdentityService
    {

        ClaimsPrincipal User { get; }
        string GetUserIdentity();
        string GetUserName();

        string GetUserMail();

        string GetUserTel();

        string GetUserFunction();

        string GetPartnersAccessType();

        bool IsAdmin();

        //bool HasAcces(string partenaireId, string userLogin);
        //bool HasAcces(string partenaireId);

        string GetPartenaireCodeOperteur();
        string GetPartenaireCode();

        string GetPartenaireNom();

        string isLimitAcces();

        bool isFromCRM();
        string GetPartenaireId();
        string GetAllUserIdsPartenaire();
        string GetName();

        bool GetPartenaireIsHelpMe();
        bool GetPartenaireHideFacturation();
        bool GetPartenaireHideGsmProfil();
        bool GetPartenaireIsNotPrelevement();
        bool GetPartenaireHasProduitAntivirus();
        bool GetPartenaireHasTrancheMsisdn();

        string GetPartenaireCdrModele();
        bool GetShowDetailFacture();
        //bool CheckAccessRight(string fonction, bool isSuperAdminFunction);
        bool CheckAccessRight(string fonction, bool isSuperAdminFunction);


    }

   

}
