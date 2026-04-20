using Hayat.BLL.Configuration;
using Hayat.BLL.Interfaces;
using Hayat.BLL.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hayat.BLL.Helper
{
    public static class BllDependencyInjection
    {
        public static IServiceCollection AddBllDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<JwtOptions>()
                .Bind(configuration.GetSection(JwtOptions.SectionName))
                .Validate(
                    options =>
                        !string.IsNullOrWhiteSpace(options.Issuer) &&
                        !string.IsNullOrWhiteSpace(options.Audience) &&
                        options.AccessTokenMinutes is >= 5 and <= 1440 &&
                        !string.IsNullOrWhiteSpace(options.Key) &&
                        options.Key.Length >= 32 &&
                        !string.Equals(options.Key, "SET_VIA_USER_SECRETS_OR_ENVIRONMENT_VARIABLE", StringComparison.Ordinal),
                    "JWT settings must be configured with a real signing key supplied via user secrets or environment variables.")
                .ValidateOnStart();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IReceptionistPortalService, ReceptionistPortalService>();
            services.AddScoped<IDoctorPortalService, DoctorPortalService>();
            services.AddScoped<IDevelopmentDataSeeder, DevelopmentDataSeeder>();

            return services;
        }
    }
}
