using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using HelpMe.Commun.Security.Identity.Abstraction;
using HelpMe.Commun.Security.Identity.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HelpMe.Commun.Security.Identity.Service
{

    public class TokenService<Tuser> : ITokenService<Tuser>
        where Tuser : ApplicationUser
    {
        private readonly IdentitySettings _identitySettings;
        readonly UserManager<Tuser> _userManager;
        public TokenService(IOptions<IdentitySettings> identitySettings,
           UserManager<Tuser> userManager
            )
        {
            _identitySettings = identitySettings.Value;
            _userManager = userManager;
        }


        public async Task<string> GenerateEmailConfirmationToken(Tuser currentUser)
        {
            var result = await _userManager.GenerateEmailConfirmationTokenAsync(currentUser);
            return result;
        }

        public async Task<string> GeneratePasswordResetToken(Tuser currentUser)
        {
            var result = await _userManager.GeneratePasswordResetTokenAsync(currentUser);
            return result;
        }
        public string GenerateAccessToken(IEnumerable<Claim> claims )
        {
           
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_identitySettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims ),
                Expires = DateTime.UtcNow.AddMinutes(_identitySettings.AccesTokenExpirationTimeMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _identitySettings.Audience,
                Issuer = _identitySettings.Issuer
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_identitySettings.SecretKey)),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
//#­307
