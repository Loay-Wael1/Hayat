using System;
using System.Collections.Generic;

namespace Hayat.DAL.Entities
{
    public class Branch
    {
        public Guid BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;

        public virtual ICollection<Clinic> Clinics { get; set; } = new List<Clinic>();
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
