using System.ComponentModel.DataAnnotations;

namespace Hayat.BLL.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string UserNameOrEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
    }
}
