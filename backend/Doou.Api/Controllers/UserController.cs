using Doou.Api.DTO.Requests;
using Doou.Api.Models;
using Doou.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Doou.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var user = await _userService.GetAllAsync();
            return Ok(user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Create(UserRequestDto dto)
        {
            var response = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new
            { id = response.Data?.UserId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(UpdateUserDto dto)
        {
            var updated = await _userService.UpdateAsync(dto);
            if (!updated.Success)
                return BadRequest(updated);

            if (updated.Data?.UserId != null)
                return CreatedAtAction(nameof(GetById), new { id = updated.Data.UserId }, updated);

            return Created(string.Empty, updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
