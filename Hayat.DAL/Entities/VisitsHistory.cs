using System;

namespace Hayat.DAL.Entities
{
    public class VisitsHistory
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PatientComplaint { get; set; }
        public string Diagnosis { get; set; }
        public string Notes { get; set; }

        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        public Guid DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }
    }
}
