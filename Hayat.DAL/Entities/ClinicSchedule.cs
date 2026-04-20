using System;
using Hayat.DAL.Entities.Enums;

namespace Hayat.DAL.Entities
{
    public class ClinicSchedule
    {
        public int ScheduleId { get; set; }
        public ClinicDayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int ClinicId { get; set; }
        public virtual Clinic Clinic { get; set; }

        public Guid DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }
    }
}
