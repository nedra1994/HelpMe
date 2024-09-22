namespace HelpMe.Commun.Infra.Mailling.Abstraction
{
    public class MaillingConfig
    {
        public string EmailAdress { get; set; }
        public string SmtpHost { get; set; }
        public int IsAutentification { get; set; }
        public int SmtpServerPort{ get; set; }
        public string EmailLogin { get; set; }
        public string EmailPassword { get; set; }
        public int EnableSsl { get; set; }
    }

   
    //public class MaillingService : IMaillingService
    //{
    //    public void Send(string from, List<string> to, string subject, string body)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
