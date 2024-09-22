using Microsoft.AspNetCore.DataProtection;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HelpMe.Commun.Security.Identity.TokenProvider
{

    public class HelpMeEmailConfirmationTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public HelpMeEmailConfirmationTokenProvider(
            IDataProtectionProvider dataProtectionProvider, 
            IOptions<HelpMeEmailConfirmationTokenProviderOptions> options,
            ILogger<DataProtectorTokenProvider<TUser>> logger)
            : base(dataProtectionProvider, options, logger)
        {
        }
    }

}