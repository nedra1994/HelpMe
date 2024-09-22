namespace HelpMe.Commun.Infra.Logging.Abstraction
{
    public interface IHelpMeLog<out TCategoryName>
    {
        public void LogTrace(string message);
        public void LogDebug(string message);
        public void LogInformation(string message);
        public void LogWarning(string message);
        public void LogError(string message);
        public void LogError(Exception ex, string message);
        public void LogCritical(string message);
        public void LogCritical(Exception ex, string message);

    }
}