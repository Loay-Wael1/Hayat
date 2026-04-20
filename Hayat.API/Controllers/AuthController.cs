using Hayat.API.Infrastructure;
using Hayat.BLL.DTOs.Auth;
using Hayat.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hayat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
        {
            var response = await _authService.LoginAsync(request, cancellationToken);
            if (response is null)
            {
                return Unauthorized(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Invalid credentials."
                });
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<CurrentUserDto>> Me(CancellationToken cancellationToken)
        {
            if (!User.TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var response = await _authService.GetCurrentUserAsync(userId, cancellationToken);
            if (response is null)
            {
                return Unauthorized();
            }

            return Ok(response);
        }
    }
}
