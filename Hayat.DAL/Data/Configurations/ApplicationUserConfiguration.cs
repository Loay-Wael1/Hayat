using Hayat.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hayat.DAL.Data.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(user => user.Id).HasValueGenerator<UuidV7Generator>();
            builder.Property(user => user.DisplayName).IsRequired().HasMaxLength(150);
            builder.Property(user => user.IsActive).HasDefaultValue(true);
            builder.Property(user => user.CreatedAt).HasDefaultValueSql("GETDATE()");

            builder.HasIndex(user => user.BranchId);
            builder.HasIndex(user => user.DoctorId)
                .IsUnique()
                .HasFilter("[DoctorId] IS NOT NULL");

            builder.HasOne(user => user.Branch)
                .WithMany(branch => branch.Users)
                .HasForeignKey(user => user.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(user => user.Doctor)
                .WithOne(doctor => doctor.User)
                .HasForeignKey<ApplicationUser>(user => user.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
