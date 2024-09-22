using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HelpMe.Commun.Security.Identity.Abstraction
{
    public interface ITokenService<Tuser> where Tuser : IUser
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        Task<string> GeneratePasswordResetToken(Tuser currentUser);
        Task<string> GenerateEmailConfirmationToken(Tuser currentUser);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
//#­307
