using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Hayat.DAL.Entities;
using Hayat.DAL.Entities.Enums;

namespace Hayat.DAL.Data.Configurations
{
    public class ClinicScheduleConfiguration : IEntityTypeConfiguration<ClinicSchedule>
    {
        public void Configure(EntityTypeBuilder<ClinicSchedule> builder)
        {
            builder.HasKey(cs => cs.ScheduleId);

            builder.HasIndex(cs => cs.ClinicId);
            builder.HasIndex(cs => cs.DoctorId);

            builder.Property(cs => cs.ScheduleId).UseIdentityColumn();
            builder.Property(cs => cs.DayOfWeek).IsRequired().HasConversion<int>();
            builder.Property(cs => cs.StartTime).IsRequired();
            builder.Property(cs => cs.EndTime).IsRequired();
        }
    }
}
