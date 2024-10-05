namespace HelpMe.Web.Identity.Api.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public DateTime DateCreation { get; set; }                                                                              
        public DateTime DateModification { get; set; }                                                                              

    }
}
