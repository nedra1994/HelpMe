
using System.Threading.Tasks;

namespace HelpMe.Commun.Security.Identity.Abstraction
{
    public interface IAccountManagerService < Tuser ,Trole> where Tuser : IUser where Trole:IRole
    {
         Task<Tuser> Authenticate(string username, string password);
         Task<Tuser> CreateAccount(Tuser currentUser, string pw);
         Task<Tuser> ConfirmeCreateAccountRequest(string username);
         Task<Tuser> ConfirmeEmailAccountRequest(string username, string token);

         Task<Tuser> ResetPasswordRequest(string username);
         Task<Tuser> ResetPassword(string username, string token, string newPw);
         Task<Tuser> ChangePassword(string username, string oldPw, string newPw);

        Task<Tuser> UpdatePartenaireUser(string userLogin, string newPartenaireId);

    }

   
}
