using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMe.Commun.Security.Identity.Abstraction
{
    public class IdentitySettings
    {
        public string SecretKey { get; set; }
        public int AccesTokenExpirationTimeMinutes { get; set; }
        public int RefreshTokenExpirationTimeMinutes { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int PasswordRequiredLength { get; set; }
        public bool  PasswordIsRequireDigit { get; set; }
        public bool PasswordIsRequireLowercase { get; set; }
        public bool PasswordIsRequireNonAlphanumeric { get; set; }
        public bool PasswordIsRequireUppercase { get; set; }
        public int PasswordMaxFailedAccessAttempts { get; set; }
        public int PasswordExpirationTimeDays { get; set; }
        public int PasswordResetTokenExpirationTimeHours { get; set; }
        public int EmailConfirmationTokenExpirationTimeHours { get; set; }
        public string ConnectionString { get; set; }
        public string ExtranetClientId { get; set; }
        public string ExtranetClientSecret { get; set; }
        public string ExtranetScope { get; set; }
        public string IdentityTokenUrl { get; set; }
        public string IdentityUrl { get; set; }
        public string ApplicationUrl { get; set; }

        public string ApplicationEspaceClientURL { get; set; }

        public string Partner_Files { get; set; }
        public string ApplicationImagePath { get; set; }

        public int ExtranetPasswordExpirationTimeDays { get; set; }
        public string WebSSHUrl { get; set; }
        //option.Password.RequiredLength = 10;
        //option.Lockout.MaxFailedAccessAttempts = 5;
    }
}
