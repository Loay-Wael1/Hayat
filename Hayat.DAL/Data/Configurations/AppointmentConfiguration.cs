using Hayat.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hayat.DAL.Data.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasKey(a => a.AppointmentId);

            builder.HasIndex(a => a.ClinicId);
            builder.HasIndex(a => a.PatientId);

            builder.Property(a => a.AppointmentId).UseIdentityColumn();
            builder.Property(a => a.AppointmentDate).IsRequired();
            builder.Property(a => a.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property(a => a.Status).IsRequired().HasConversion<string>().HasMaxLength(50);
        }
    }
}
