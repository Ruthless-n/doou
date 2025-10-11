namespace Doou.Api.DTO.Requests
{
    public class CreateUserRequestDto
    {
        public string? Name { get; set; }
        public string? CPF { get; set; }
        public string? Address { get; set; }
        public DateTime BirthDate { get; set; }

        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
