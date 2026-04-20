using Hayat.DAL.Data;
using Hayat.DAL.Interfaces;

namespace Hayat.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HayatDbContext _context;

        public UnitOfWork(HayatDbContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
