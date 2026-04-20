using System;
using System.Collections.Generic;

namespace Hayat.DAL.Entities
{
    public class Clinic
    {
        public int ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;

        public Guid BranchId { get; set; }
        public virtual Branch Branch { get; set; } = null!;

        public virtual ICollection<ClinicSchedule> ClinicSchedules { get; set; } = new List<ClinicSchedule>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
