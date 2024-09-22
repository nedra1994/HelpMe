namespace HelpMe.Commun.Infra.Mailling.Abstraction
{
    public interface IMaillingService
    {
        void Send(Mail mail);
        void SendFTM(Mail mail);

    }

}
