using Microsoft.AspNetCore.Identity;

namespace Hayat.DAL.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string DisplayName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid? BranchId { get; set; }
        public virtual Branch? Branch { get; set; }

        public Guid? DoctorId { get; set; }
        public virtual Doctor? Doctor { get; set; }
    }
}
