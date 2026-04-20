using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Hayat.DAL.Entities;

namespace Hayat.DAL.Data.Configurations
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(d => d.DoctorId);

            builder.HasIndex(d => d.FullName);

            builder.Property(d => d.DoctorId).HasValueGenerator<UuidV7Generator>();
            builder.Property(d => d.FullName).IsRequired().HasMaxLength(150);
            builder.Property(d => d.Specialty).IsRequired().HasMaxLength(100);

            builder.HasMany(d => d.ClinicSchedules)
                   .WithOne(cs => cs.Doctor)
                   .HasForeignKey(cs => cs.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(d => d.CreatedVisitsHistory)
                   .WithOne(mh => mh.Doctor)
                   .HasForeignKey(mh => mh.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
