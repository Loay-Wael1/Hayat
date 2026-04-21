using Hayat.BLL.DTOs.Receptionist;
using Hayat.BLL.DTOs.Shared;

namespace Hayat.BLL.Interfaces
{
    public interface IReceptionistPortalService
    {
        Task<IReadOnlyList<PatientSearchResultDto>> SearchPatientsAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<AppointmentSummaryDto> BookAppointmentAsync(Guid branchId, BookAppointmentRequestDto request, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<AppointmentSummaryDto>> GetAppointmentsForDateAsync(Guid branchId, DateOnly date, CancellationToken cancellationToken = default);
        Task<RegisterPatientResponseDto> RegisterPatientAsync(RegisterPatientRequestDto request, CancellationToken cancellationToken = default);
    }
}
