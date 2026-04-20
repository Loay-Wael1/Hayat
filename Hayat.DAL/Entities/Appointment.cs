using System;
using System.Collections.Generic;
using Hayat.DAL.Entities.Enums;

namespace Hayat.DAL.Entities
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public AppointmentStatus Status { get; set; }

        public int ClinicId { get; set; }
        public virtual Clinic Clinic { get; set; }

        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; }

    }
}
