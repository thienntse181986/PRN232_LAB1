using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Generic;
using PRN232.LMS.Services.Helpers;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Request;
using PRN232.LMS.Services.Models.Response;

namespace PRN232.LMS.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _unitOfWork.Users.GetByUsernameAsync(request.Username);
        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return ApiResponse<LoginResponse>.Fail("Invalid username or password.");
        }

        var accessToken = GenerateAccessToken(user, out int expiresIn);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        var refreshExpiryDays = int.TryParse(_configuration["Jwt:RefreshTokenExpiryDays"], out var rDays) ? rDays : 7;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshExpiryDays);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<LoginResponse>.Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = expiresIn
        }, "Logged in successfully.");
    }

    public async Task<ApiResponse<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var principal = GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
        {
            return ApiResponse<LoginResponse>.Fail("Invalid access token.");
        }

        var username = principal.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(username))
        {
            return ApiResponse<LoginResponse>.Fail("Invalid access token claims.");
        }

        var user = await _unitOfWork.Users.GetByUsernameAsync(username);
        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return ApiResponse<LoginResponse>.Fail("Invalid or expired refresh token.");
        }

        var newAccessToken = GenerateAccessToken(user, out int expiresIn);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        var refreshExpiryDays = int.TryParse(_configuration["Jwt:RefreshTokenExpiryDays"], out var rDays) ? rDays : 7;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshExpiryDays);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<LoginResponse>.Ok(new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = expiresIn
        }, "Token refreshed successfully.");
    }

    private string GenerateAccessToken(User user, out int expiresInSeconds)
    {
        var secret = Environment.GetEnvironmentVariable("JWT_SECRET")
                     ?? _configuration["Jwt:Secret"]
                     ?? "FallbackSuperSecretJWTKey12345678901234567890";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var issuer = _configuration["Jwt:Issuer"] ?? "LmsApi";
        var audience = _configuration["Jwt:Audience"] ?? "LmsClient";
        var expiryMinutes = int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var expMin) ? expMin : 60;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds
        );

        expiresInSeconds = expiryMinutes * 60;
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var secret = Environment.GetEnvironmentVariable("JWT_SECRET")
                     ?? _configuration["Jwt:Secret"]
                     ?? "FallbackSuperSecretJWTKey12345678901234567890";

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateLifetime = false // Here we ignore token's expiration date!
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            return principal;
        }
        catch
        {
            return null;
        }
    }
}
