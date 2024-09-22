

using Microsoft.AspNetCore.Identity;
using HelpMe.Commun.Security.Identity.Abstraction;
using System;
using System.Collections.Generic;

namespace HelpMe.Commun.Security.Identity.Data
{
    public abstract class ApplicationUser : IdentityUser, IUser
    {
        public abstract bool? IsDisabled { get; set; }
        public abstract bool? IsAccountRequestConfirmed { get; set; }
        public abstract string AccountRequestConfirmedBy { get; set; }
        public abstract DateTime? AccountRequestConfirmedDate { get; set; }
        public abstract DateTime? ResetPasswordRequestDate { get; set; }
        public abstract DateTime? ExpirationDate { get; set; }
        public abstract string Token { get; set; }
        public abstract string  Function { get; set; }
        public abstract string ResetPasswordRequestToken { get; set; }
        public abstract string AccountRequestConfirmedToken { get; set; }
        public abstract string Name { get; set; }
        public abstract string PartenaireId { get; set; }
        public abstract string ClientCode { get; set; }

        public abstract string PartenaireParentId { get; set; }
        public abstract string PartnersAccessType { get; set; }

        public abstract DateTime? LastLoginDate { get; set; }
        public abstract bool? isDoubleAuth { get; set; }
        public abstract string firstName { get; set; }
        public abstract string? CodeConfirmation { get; set; }
        public abstract DateTime? DateCreateCode { get; set; }
        public abstract DateTime? DateCreation { get; set; }
        public abstract DateTime? DateModification { get; set; }
        //public abstract IReadOnlyCollection<RefreshToken> RefreshTokens { get;  }
        //public abstract bool HasValidRefreshToken(string refreshToken);
        //public abstract void AddRefreshToken(string token, string remoteIpAddress, double MinutesToExpire);
        //public abstract void RemoveRefreshToken(string refreshToken);

    }
   
}
