using Doou.Api.DTO.Requests;
using Doou.Api.Models;

namespace Doou.Api.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(CreateUserRequestDto dto);
        Task<User?> UpdateAsync(int id, User user);
        Task<bool> DeleteAsync(int id);
    }
}
