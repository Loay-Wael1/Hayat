using Hayat.DAL.Entities;

namespace Hayat.DAL.Interfaces
{
    public interface IVisitsHistoryRepository : IGenericRepository<VisitsHistory>
    {
        Task<IReadOnlyList<VisitsHistory>> GetPatientTimelineAsync(Guid patientId, CancellationToken cancellationToken = default);
    }
}
