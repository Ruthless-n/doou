using Doou.Api.Config;
using Doou.Api.DTO.Requests;
using Doou.Api.DTO.Responses;
using Doou.Api.Models;
using Doou.Api.Models.Responses;
using Doou.Api.Helpers;
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

        public async Task<ApiResponse<User>> CreateAsync(UserRequestDto dto)
        {
            try
            {
                if (await _dbContext.Users.AnyAsync(u => u.Email == dto.Email))
                {
                    return new ApiResponse<User>
                    {
                        Success = false,
                        Message = ErrorMessages.User.EmailAlreadyExists,
                        StatusCode = 400,
                        Data = null
                    };
                }

                if (await _dbContext.Users.AnyAsync(u => u.CPF == dto.CPF))
                {
                    return new ApiResponse<User>
                    {
                        Success = false,
                        Message = ErrorMessages.User.CPFAlreadyExists,
                        StatusCode = 400,
                        Data = null
                    };
                }

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
                return new ApiResponse<User>
                {
                    Success = true,
                    Message = SuccessMessages.User.UserCreated,
                    StatusCode = 201,
                    Data = new UserResponseDto
                    {
                        UserId = user.UserId,
                        Name = user.Name,
                        Email = user.Email,
                        Address = user.Address,
                        Password = user.Password
                    }
                };
            }

            catch (Exception ex)
            {
                return new ApiResponse<User>
                {
                    Success = false,
                    Message = ErrorMessages.General.InternalServerError + " " + ex.Message,
                    StatusCode = 500,
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<User>> UpdateAsync(UpdateUserDto dto)
        {
            try
            {
                if(await _dbContext.Users.AnyAsync(u => u.CPF == dto.CPF && u.UserId != dto.UserId))
                {
                    return new ApiResponse<User>
                    {
                        Success = false,
                        Message = ErrorMessages.User.CPFAlreadyExists,
                        StatusCode = 400,
                        Data = null
                    };
                }

                if (await _dbContext.Users.AnyAsync(u => u.Email == dto.Email && u.UserId != dto.UserId))
                {
                    return new ApiResponse<User>
                    {
                        Success = false,
                        Message = ErrorMessages.User.EmailAlreadyExists,
                        StatusCode = 400,
                        Data = null
                    };
                }

                var user = await _dbContext.Users.FindAsync(dto.UserId);
                
                if (user == null)
                {
                    return new ApiResponse<User>
                    {
                        Success = false,
                        Message = ErrorMessages.User.UserNotFound,
                        StatusCode = 404,
                        Data = null
                    };
                }

                user.Name = dto.Name;
                user.CPF = dto.CPF;
                user.Address = dto.Address;
                user.BirthDate = DateTime.SpecifyKind(dto.BirthDate, DateTimeKind.Utc);
                user.Email = dto.Email;

                await _dbContext.SaveChangesAsync();

                return new ApiResponse<User>
                {
                    Success = true,
                    Message = SuccessMessages.User.UserUpdated,
                    StatusCode = 200,
                    Data = user
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<User>
                {
                    Success = false,
                    Message = ErrorMessages.General.InternalServerError + " " + ex.Message,
                    StatusCode = 500,
                    Data = null
                };
            }
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
