using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMe.Commun.Security.Identity.Abstraction
{
    public class HelpMeAuthenticationException:Exception
    {
      //  public readonly eAuthenticationFailureCause? FailureCause;
        public HelpMeAuthenticationException(string message) : base(message)
        {
            //FailureCause = failureCause;
        }

       
    }

    public class HelpMeCreateAccountException : Exception
    {
        public HelpMeCreateAccountException(string  message) : base(message)
        {
           
        }

    }
    public class HelpMeRefreshTokenException : Exception
    {
        public HelpMeRefreshTokenException(string message) : base(message)
        {

        }

    }

    public class HelpMeRevokeTokenException : Exception
    {
        public HelpMeRevokeTokenException(string message) : base(message)
        {

        }

    }
    


    public class AccountComfirmeException : Exception
    {
        public AccountComfirmeException(string message) : base(message)
        {

        }

    }
    public class AccountComfirmeMailException : Exception
    {
        public AccountComfirmeMailException(string message) : base(message)
        {

        }

    }

    public class ResetPasswordRequestException : Exception
    {
        public ResetPasswordRequestException(string message) : base(message)
        {

        }

    }
    

    public class ResetPasswordException : Exception
    {
        public ResetPasswordException(string message) : base(message)
        {

        }

    }
    
    public class ChangePasswordException : Exception
    {
        public ChangePasswordException(string message) : base(message)
        {

        }

    }
    

    public enum eAuthenticationFailureCause
    {

        NotAllowed,
        InvalidePassword,
        NotEmailConfirmed,
        LockedOut,
        Disabled,
        PasswordExpired,
    }

}
