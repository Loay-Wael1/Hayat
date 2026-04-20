using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Hayat.BLL.Configuration;
using Hayat.BLL.DTOs.Auth;
using Hayat.BLL.Interfaces;
using Hayat.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Hayat.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtOptions _jwtOptions;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
        {
            var user = await FindUserWithProfileAsync(request.UserNameOrEmail, cancellationToken);
            if (user is null || !user.IsActive)
            {
                return null;
            }

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault() ?? string.Empty;

            return new AuthResponseDto
            {
                UserId = user.Id,
                DisplayName = user.DisplayName,
                Role = primaryRole,
                AccessToken = GenerateAccessToken(user, roles),
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                DoctorId = user.DoctorId,
                BranchId = user.BranchId
            };
        }

        public async Task<CurrentUserDto?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users
                .AsNoTracking()
                .Where(existingUser => existingUser.Id == userId)
                .SingleOrDefaultAsync(cancellationToken);

            if (user is null || !user.IsActive)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new CurrentUserDto
            {
                UserId = user.Id,
                DisplayName = user.DisplayName,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty,
                DoctorId = user.DoctorId,
                BranchId = user.BranchId
            };
        }

        private async Task<ApplicationUser?> FindUserWithProfileAsync(string loginIdentifier, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(loginIdentifier))
            {
                return null;
            }

            var normalizedIdentifier = loginIdentifier.Trim().ToUpperInvariant();

            return await _userManager.Users
                .AsNoTracking()
                .Include(user => user.Doctor)
                .Include(user => user.Branch)
                .FirstOrDefaultAsync(user =>
                    user.NormalizedUserName == normalizedIdentifier ||
                    user.NormalizedEmail == normalizedIdentifier,
                    cancellationToken);
        }

        private string GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.DisplayName),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            if (user.DoctorId.HasValue)
            {
                claims.Add(new Claim("doctorId", user.DoctorId.Value.ToString()));
            }

            if (user.BranchId.HasValue)
            {
                claims.Add(new Claim("branchId", user.BranchId.Value.ToString()));
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
