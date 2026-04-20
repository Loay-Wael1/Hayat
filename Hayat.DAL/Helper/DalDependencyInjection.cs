using Hayat.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hayat.DAL.Helper
{
    public static class DalDependencyInjection
    {
        public static IServiceCollection AddDalDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<HayatDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}
