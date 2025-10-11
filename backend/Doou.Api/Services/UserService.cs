using Doou.Api.Config;
using Doou.Api.DTO.Requests;
using Doou.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Doou.Api.Services
{
    public class UserService : IUserService
    {
        private readonly DoouDbContext _dbContext;

        public UserService(DoouDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(d => d.UserId == id);
        }

        public async Task<User> CreateAsync(CreateUserRequestDto dto)
        {
            var user = new User
            {
                Name = dto.Name,
                CPF = dto.CPF,
                Address = dto.Address,
                BirthDate = DateTime.SpecifyKind(dto.BirthDate, DateTimeKind.Utc),
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateAsync(int id, User user)
        {
            var existing = await _dbContext.Users.FindAsync(id);
            if (existing == null) return null;

            existing.Name = user.Name;
            existing.Email = user.Email;
            existing.Address = user.Address;
            existing.BirthDate = user.BirthDate;
            existing.CPF = user.CPF;
            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _dbContext.Users.FindAsync(id);
            if (existing == null) return false;

            _dbContext.Users.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
