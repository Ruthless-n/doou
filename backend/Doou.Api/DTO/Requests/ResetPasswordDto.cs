namespace Doou.Api.DTO.Requests
{
    public class ResetPasswordDto
    {
        public int Token { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
