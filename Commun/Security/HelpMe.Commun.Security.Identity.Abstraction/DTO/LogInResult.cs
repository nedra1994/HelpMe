namespace HelpMe.Commun.Security.Identity.Abstraction
{
    public class LogInResult
    {

        public readonly bool IsLockedOut;
        public readonly bool IsNotAllowed;

        public LogInResult(bool isLockedOut, bool isNotAllowed)
        {
            IsLockedOut = isLockedOut;
            IsNotAllowed = isNotAllowed;
        }
    }
}
//#­307
