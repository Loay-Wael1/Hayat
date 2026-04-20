using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Hayat.DAL.Entities;

namespace Hayat.DAL.Data.Configurations
{
    public class VisitsHistoryConfiguration : IEntityTypeConfiguration<VisitsHistory>
    {
        public void Configure(EntityTypeBuilder<VisitsHistory> builder)
        {
            builder.HasKey(vh => vh.Id);

            builder.HasIndex(vh => vh.PatientId);
            builder.HasIndex(vh => vh.DoctorId);

            builder.Property(vh => vh.Id).HasValueGenerator<UuidV7Generator>();
            builder.Property(vh => vh.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property(vh => vh.PatientComplaint).IsRequired().HasColumnType("nvarchar(max)");
            builder.Property(vh => vh.Diagnosis).HasColumnType("nvarchar(max)");
            builder.Property(vh => vh.Notes).HasColumnType("nvarchar(max)");

            builder.HasOne(vh => vh.Patient)
                   .WithMany(p => p.VisitsHistory)
                   .HasForeignKey(vh => vh.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(vh => vh.Doctor)
                   .WithMany(d => d.CreatedVisitsHistory)
                   .HasForeignKey(vh => vh.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
