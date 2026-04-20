using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Hayat.DAL.Data
{
    public class HayatDbContext: DbContext
    {
        public HayatDbContext(DbContextOptions<HayatDbContext> options): base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        base.OnModelCreating(modelBuilder);

        // Apply all IEntityTypeConfiguration<T> classes from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }


    }
