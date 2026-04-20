using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Hayat.DAL.Entities;

namespace Hayat.DAL.Data.Configurations
{
    public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
    {
        public void Configure(EntityTypeBuilder<Clinic> builder)
        {
            builder.HasKey(c => c.ClinicId);

            builder.HasIndex(c => c.BranchId);

            builder.Property(c => c.ClinicId).UseIdentityColumn();
            builder.Property(c => c.ClinicName).IsRequired().HasMaxLength(150);

            // Composition Relationship explicitly requesting cascade delete
            builder.HasMany(c => c.ClinicSchedules)
                   .WithOne(cs => cs.Clinic)
                   .HasForeignKey(cs => cs.ClinicId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Appointments)
                   .WithOne(a => a.Clinic)
                   .HasForeignKey(a => a.ClinicId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
