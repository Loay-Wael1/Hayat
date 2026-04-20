using Hayat.DAL.Entities;

namespace Hayat.DAL.Interfaces
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<IReadOnlyList<Appointment>> GetAppointmentsByBranchAndDateAsync(Guid branchId, DateOnly date, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Appointment>> GetDoctorQueueAsync(Guid doctorId, DateOnly date, CancellationToken cancellationToken = default);
    }
}
