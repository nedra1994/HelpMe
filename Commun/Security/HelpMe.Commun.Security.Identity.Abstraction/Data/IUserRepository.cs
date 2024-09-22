
using HelpMe.commun.domain.Data.Repositories;
using HelpMe.Commun.Security.Identity.Abstraction;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpMe.Commun.Security.Identity.Abstraction
{
    public interface IUserRepository<Tuser,Trole>  : IRepository<Tuser>
        where Tuser :  IUser
        where Trole : IRole

    {
        Task<Tuser> FindByName(string userName);
        Task<Result<Tuser>> Create(Tuser currentUser, string pw);
        Task<Result<Tuser>> ConfirmEmail(Tuser currentUser, string token);
        Task<Result<Tuser>> ChangePassword(Tuser currentUser, string oldPw, string newPw);
        Task<Result<Tuser>> ResetPassword(Tuser currentUser, string token, string newPw);
        Task<Result<LogInResult>> CheckPasswordSignIn(Tuser currentUser, string password, bool lockedOut);
        Task<Result<Tuser>> UpdateUser(Tuser currentUser);


        Task<List<string>> GetPartenaireFonctinalites(Tuser currentUser);
        //Task<string> GenerateEmailConfirmationToken(Tuser currentUser);
        Task<List<string>> GetFonctinalitesSU();
        Task<List<string>> GetAllFonctinalitesNotSU();

    }
}
