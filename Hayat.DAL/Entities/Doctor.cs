using System;
using System.Collections.Generic;

namespace Hayat.DAL.Entities
{
    public class Doctor
    {
        public Guid DoctorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;

        public virtual ICollection<ClinicSchedule> ClinicSchedules { get; set; } = new List<ClinicSchedule>();
        public virtual ICollection<VisitsHistory> CreatedVisitsHistory { get; set; } = new List<VisitsHistory>();
        public virtual ApplicationUser? User { get; set; }
    }
}
