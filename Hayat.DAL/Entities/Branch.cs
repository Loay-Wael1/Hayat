using System;
using System.Collections.Generic;

namespace Hayat.DAL.Entities
{
    public class Branch
    {
        public Guid BranchId { get; set; }
        public string BranchName { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }

        public virtual ICollection<Clinic> Clinics { get; set; } = new List<Clinic>();
    }
}
