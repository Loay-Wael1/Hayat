using Hayat.BLL.DTOs.Receptionist;
using Hayat.BLL.DTOs.Shared;

namespace Hayat.BLL.Interfaces
{
    public interface IReceptionistPortalService
    {
        Task<IReadOnlyList<PatientSearchResultDto>> SearchPatientsAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<AppointmentSummaryDto> QuickBookAsync(Guid branchId, QuickBookAppointmentRequestDto request, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<AppointmentSummaryDto>> GetAppointmentsForDateAsync(Guid branchId, DateOnly date, CancellationToken cancellationToken = default);
    }
}
