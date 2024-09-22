using Microsoft.AspNetCore.DataProtection;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HelpMe.Commun.Security.Identity
{
    public class HelpMePasswordResetTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public HelpMePasswordResetTokenProvider(
            IDataProtectionProvider dataProtectionProvider,
            IOptions<HelpMePasswordResetTokenProviderOptions> options,
            ILogger<DataProtectorTokenProvider<TUser>> logger)
            : base(dataProtectionProvider, options, logger)
        {
        }
    }

}