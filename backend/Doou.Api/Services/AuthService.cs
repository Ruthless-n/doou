using Doou.Api.Config;
using Doou.Api.DTO.Requests;
using Doou.Api.Helpers;
using Doou.Api.Models;
using Doou.Api.Models.Responses;
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

        public Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == dto.Email);

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
            var key = _configuration["JWT_SECRET"];
            var issuer = _configuration["JWT_ISSUER"];
            var audience = _configuration["JWT_AUDIENCE"];
            var expiryMinutesStr = _configuration["JWT_EXPIRATION_MINUTES"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var jwtToken = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(expiryMinutesStr)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
        private void SendEmail(string to, string subject, string body)
        {
            var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST");
            var smtpPortStr = Environment.GetEnvironmentVariable("SMTP_PORT");
            var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER");
            var smtpPass = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

            try
            {
                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpPortStr)
                    || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
                {
                    throw new Exception("Configurações SMTP ausentes no .env");
                }

                if (!int.TryParse(smtpPortStr, out int smtpPort))
                {
                    throw new Exception("SMTP_PORT não é um número válido.");
                }

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new System.Net.NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                var mail = new MailMessage(smtpUser, to, subject, body);
                client.Send(mail);

            }

            catch (SmtpException ex)
            {
                throw new Exception($"Erro SMTP: {ex.StatusCode} - {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro inesperado ao enviar e-mail: {ex.Message}");
            }
        }

    }
}
