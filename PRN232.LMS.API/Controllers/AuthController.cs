using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Request;
using PRN232.LMS.Services.Models.Response;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
[Produces("application/json", "application/xml")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Logs in a user and returns JWT access and refresh tokens</summary>
    /// <param name="request">Login request credentials</param>
    /// <response code="200">User authenticated successfully</response>
    /// <response code="400">Invalid username or password, or model validation failed</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Validation failed.", ModelState));
        }

        var result = await _authService.LoginAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>Refreshes an expired JWT access token using a valid refresh token</summary>
    /// <param name="request">Refresh token request containing old access token and refresh token</param>
    /// <response code="200">Token refreshed successfully</response>
    /// <response code="400">Invalid or expired refresh token, or model validation failed</response>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Validation failed.", ModelState));
        }

        var result = await _authService.RefreshTokenAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
