using Doou.Api.DTO.Requests;
using Doou.Api.Models;
using Doou.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Doou.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(AuthService userService)
        {
            _authService = (IAuthService)userService;
        }

        /// <summary>
        /// Faz o login do usuário e retorna um token JWT.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Solicita redefinição de senha. Gera token de 6 dígitos e envia por e-mail.
        /// </summary>
        [HttpPost("forgot-request")]
        public async Task<IActionResult> ResetPasswordRequest([FromBody] string email)
        {
            var result = await _authService.ForgotPasswordAsync(email);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        /// <summary>
        /// Redefine a senha informando o token recebido por e-mail.
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetDto)
        {
            var result = await _authService.ResetPasswordAsync(resetDto);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}
