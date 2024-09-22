using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HelpMe.commun.domain.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMe.Commun.Security.Identity.Data
{
    public class ApplicationDbContext<Tuser,Trole> : IdentityDbContext<Tuser,Trole,string>, IUnitOfWork
        where Tuser : ApplicationUser
        where Trole : ApplicationRole
    {
        //public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        //    : base(options)
        //{
        //}

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }

        //public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Tuser>(ConfigureUser);
            builder.Entity<Trole>(ConfigureRole);
            //builder.Entity<IdentityRole>().ToTable("Roles");

            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");

            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

        }

        public void ConfigureUser(EntityTypeBuilder<Tuser> modelBuilder)
        {
           
            modelBuilder.ToTable("Users");
            //var navigation = modelBuilder.Metadata.FindNavigation("RefreshTokens");
            ////EF access the RefreshTokens collection property through its backing field
            //navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }

        public void ConfigureRole(EntityTypeBuilder<Trole> modelBuilder)
        {
            modelBuilder.ToTable("Roles");
            //var navigation = modelBuilder.Metadata.FindNavigation("RefreshTokens");
            ////EF access the RefreshTokens collection property through its backing field
            //navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
           

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed through the DbContext will be committed
            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}