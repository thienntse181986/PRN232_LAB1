using PRN232.LMS.Services.Models.Request;
using PRN232.LMS.Services.Models.Response;

namespace PRN232.LMS.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request);
}
