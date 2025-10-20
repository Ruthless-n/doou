using Doou.Api.Config;
using Doou.Api.DTO.Requests;
using Doou.Api.Helpers;
using Doou.Api.Models;
using Doou.Api.Models.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Doou.Api.Services
{
    public class AuthService : IAuthService
    {

        private readonly DoouDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthService(DoouDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        private string Generate6DigitToken()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public Task<ApiResponse<string>> LoginAsync(LoginDto dto)
        {
            try
            {
                var user = _dbContext.Users
                    .FirstOrDefault(u => u.Email == dto.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                {
                    return Task.FromResult(new ApiResponse<string>
                    {
                        Success = false,
                        Message = ErrorMessages.Auth.InvalidCredentials,
                        StatusCode = 401,
                        Data = null
                    });
                }

                var token = string.Empty;
                token = GenerateJwtToken(user);

                return Task.FromResult(new ApiResponse<string>
                    {
                        Success = true,
                        Message = SuccessMessages.Auth.UserLoggedIn,
                        StatusCode = 200,
                        Data = token
                    });
            }

            catch (Exception ex)
            {
                return Task.FromResult(new ApiResponse<string>
                {
                    Success = false,
                    Message = ErrorMessages.General.InternalServerError + " " + ex.Message,
                    StatusCode = 500,
                    Data = null
                });
            }
        }

        public Task<ApiResponse<bool>> ForgotPasswordAsync(string email)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return Task.FromResult(new ApiResponse<bool>
                {
                    Success = false,
                    Message = ErrorMessages.Auth.UserNotFound,
                    StatusCode = 404,
                    Data = false
                });
            }

            var token = Generate6DigitToken();

            SendEmail(user.Email, "Password Reset Token", $"Your password reset token is: {token}");

            return Task.FromResult(new ApiResponse<bool>
            {
                Success = true,
                Message = SuccessMessages.Auth.PasswordResetEmailSent,
                StatusCode = 200,
                Data = true
            });
        }

        public Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.ResetPasswordToken == dto.Token);

            if (user == null || user.ResetPasswordExpiration < DateTime.UtcNow)
            {
                return Task.FromResult(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Token inválido ou expirado.",
                    StatusCode = 400,
                    Data = false
                });
            }

            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return Task.FromResult(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "As senhas não coincidem.",
                    StatusCode = 400,
                    Data = false
                });
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.ResetPasswordToken = 0;
            user.ResetPasswordExpiration = null;

            _dbContext.SaveChanges();

            return Task.FromResult(new ApiResponse<bool>
            {
                Success = true,
                Message = "Senha atualizada com sucesso.",
                StatusCode = 200,
                Data = true
            });
        }


        private string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var claims = new[]
            {
                new Claim("id", user.UserId.ToString()),
                new Claim(ClaimTypes.Email, value: user.Email)
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var jwtToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            ); 

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
        private void SendEmail(string to, string subject, string body)
        {
            var smtpHost = _configuration["Smtp:Host"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"]);
            var smtpUser = _configuration["Smtp:User"];
            var smtpPass = _configuration["Smtp:Password"];

            using var client = new SmtpClient(smtpHost, smtpPort);
            client.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPass);
            client.EnableSsl = true;

            var mail = new MailMessage(smtpUser, to, subject, body);
            client.Send(mail);
        }
    }
}
