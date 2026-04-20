using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Hayat.API.Infrastructure
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool TryGetUserId(this ClaimsPrincipal principal, out Guid userId)
        {
            return TryGetGuidClaim(principal, ClaimTypes.NameIdentifier, out userId);
        }

        public static bool TryGetDoctorId(this ClaimsPrincipal principal, out Guid doctorId)
        {
            return TryGetGuidClaim(principal, "doctorId", out doctorId);
        }

        public static bool TryGetBranchId(this ClaimsPrincipal principal, out Guid branchId)
        {
            return TryGetGuidClaim(principal, "branchId", out branchId);
        }

        private static bool TryGetGuidClaim(ClaimsPrincipal principal, string claimType, out Guid value)
        {
            var rawValue = principal.FindFirstValue(claimType);
            return Guid.TryParse(rawValue, out value);
        }
    }
}
