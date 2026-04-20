using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hayat.BLL.Helper
{
    public static class BllDependencyInjection
    {
        public static IServiceCollection AddBllDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // Register BLL dependencies here (Services, Managers, etc.)

            return services;
        }
    }
}
