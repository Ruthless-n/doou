namespace Doou.Api.DTO.Requests
{
    public class UpdateUserDto
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? CPF { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Address { get; set; }
    }
}
