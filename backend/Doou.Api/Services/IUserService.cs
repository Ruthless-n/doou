using Doou.Api.DTO.Requests;
using Doou.Api.Models;
using Doou.Api.Models.Responses;

namespace Doou.Api.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<ApiResponse<User>> CreateAsync(UserRequestDto dto);
        Task<ApiResponse<User>> UpdateAsync(UpdateUserDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
