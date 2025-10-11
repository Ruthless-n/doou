using Doou.Api.DTO.Requests;
using Doou.Api.Models;
using Doou.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Doou.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateUserRequestDto dto)
        {
            var response = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new 
            { id = response.UserId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User user)
        {
            var updated = await _userService.UpdateAsync(id, user);
            if (updated == null) return NotFound();
            return Ok(updated);
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
