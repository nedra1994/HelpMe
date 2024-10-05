using HelpMe.Commun.Security.Identity.Data;

namespace HelpMe.Web.Identity.Api.Models
{
    public class RoleUser : ApplicationRole
    {
        public string RoleLabel { get; set; }
        public bool? IsDefault { get; set; }
        public DateTime? DateCreation { get; set; }
        public DateTime? DateModification { get; set; }
    }
}
