using Doou.Api.DTO.Requests;
using Doou.Api.Models.Responses;

namespace Doou.Api.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> LoginAsync(LoginDto dto); 
        Task<ApiResponse<bool>> ForgotPasswordAsync(string email);
        Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordDto dto);
    }
}
