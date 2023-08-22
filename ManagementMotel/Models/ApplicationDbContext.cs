using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ManagementMotel.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<UserDetail> UserDetails { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.ResetPasswordToken).IsRequired(false);
                entity.HasOne(e => e.UserDetail).WithOne(e => e.User).HasForeignKey<ApplicationUser>(e => e.UserDetailId);
            });
            builder.Entity<UserDetail>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CCCD).IsRequired().HasMaxLength(12);
                entity.Property(e => e.Address).IsRequired();
                entity.Property(e => e.MobilePhone).IsRequired().HasMaxLength(11);
                entity.Property(e => e.ImagePreviousCCCD).IsRequired();
                entity.Property(e => e.ImageAfterCCCD).IsRequired();
            });
        }
    }
}
