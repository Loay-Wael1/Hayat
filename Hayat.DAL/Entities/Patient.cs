using System;
using System.Collections.Generic;
using Hayat.DAL.Entities.Enums;

namespace Hayat.DAL.Entities
{
    public class Patient
    {
        public Guid PatientId { get; set; }
        public string FullName { get; set; }
        public string NationalId { get; set; }
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<VisitsHistory> VisitsHistory { get; set; } = new List<VisitsHistory>();
    }
}
