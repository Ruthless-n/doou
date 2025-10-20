using Doou.Api.Models;

namespace Doou.Api.DTO.Responses
{
    internal class UserResponseDto : User
    {
        public new int UserId { get; set; }
        public new string Name { get; set; }
    }
}