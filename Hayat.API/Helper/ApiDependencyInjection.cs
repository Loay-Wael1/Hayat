using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Hayat.API.Helper
{
    public static class ApiDependencyInjection
    {
        public static IServiceCollection AddApiDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // Register API specific dependencies here

            return services;
        }
    }
}
