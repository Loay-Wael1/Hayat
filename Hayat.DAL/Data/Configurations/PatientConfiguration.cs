using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Hayat.DAL.Entities;
using Hayat.DAL.Entities.Enums;

namespace Hayat.DAL.Data.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasKey(p => p.PatientId);

            builder.HasIndex(p => p.NationalId).IsUnique();
            builder.HasIndex(p => p.Phone);

            builder.Property(p => p.PatientId).HasValueGenerator<UuidV7Generator>();
            builder.Property(p => p.FullName).IsRequired().HasMaxLength(150);
            builder.Property(p => p.NationalId).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Gender)
                   .HasConversion(
                       g => g == Gender.Male,
                       b => b ? Gender.Male : Gender.Female
                   );
            builder.Property(p => p.Phone).HasMaxLength(20);
            builder.Property(p => p.Address).HasMaxLength(250);
            builder.Property(p => p.DateOfBirth).IsRequired();

            // Composition Relationship explicitly requesting cascade delete
            builder.HasMany(p => p.Appointments)
                   .WithOne(a => a.Patient)
                   .HasForeignKey(a => a.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
