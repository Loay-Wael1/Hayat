using Hayat.DAL.Entities;

namespace Hayat.DAL.Interfaces
{
    public interface IPatientRepository : IGenericRepository<Patient>
    {
        Task<IReadOnlyList<Patient>> SearchAsync(string searchTerm, int take = 20, CancellationToken cancellationToken = default);
    }
}
