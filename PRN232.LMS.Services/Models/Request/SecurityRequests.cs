using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.Models.Request;

public class LoginRequest
{
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "Access token is required.")]
    public string AccessToken { get; set; } = string.Empty;

    [Required(ErrorMessage = "Refresh token is required.")]
    public string RefreshToken { get; set; } = string.Empty;
}
