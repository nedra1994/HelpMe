using HelpMe.Commun.Security.Identity.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HelpMe.Web.Identity.Api.Models
{
    public class HelpMeResellerUser : ApplicationUser
    {
        private readonly ILazyLoader _lazyLoader;
        public HelpMeResellerUser()
        {

        }
        public HelpMeResellerUser(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }
        public User User { 

            get => _lazyLoader.Load(this, ref _User);
            set => _User = value;
        }

        private User _User ;

        [Required]
        public override string Name { get; set; }

        [Required]
        public override string Function { get; set; }

        //[Required]
        //public override string PartenaireId { get; set; }

        [Required]
        public override bool? IsDisabled { get; set; }

        public override DateTime? LastLoginDate { get; set; }
        public override string? CodeConfirmation { get; set; }
        public override DateTime? DateCreateCode { get; set; }
        public override DateTime? DateCreation { get; set; }
        public override DateTime? DateModification { get; set; }

        public override bool? isDoubleAuth { get; set; }

        public override bool? IsAccountRequestConfirmed { get; set; }
        public override string AccountRequestConfirmedBy { get; set; }
        public override DateTime? AccountRequestConfirmedDate { get; set; }
        public override DateTime? ResetPasswordRequestDate { get; set; }

        [Required]
        public override DateTime? ExpirationDate { get; set; }
        [NotMapped]
        public override string Token { get; set; }

        [NotMapped]
        public override string ResetPasswordRequestToken { get; set; }
        [NotMapped]
        public override string AccountRequestConfirmedToken { get; set; }

        public override string UserId { get; set; }

        public override string ClientCode { get; set; }

        public override string UsersAccessType { get; set; }
        public override string firstName { get; set; }
    }
}
