using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Doou.Api.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        public string? Name { get; set; }
        [MaxLength(11)]

        [Required]
        public string? CPF { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public required string Address { get; set; }

        [Required]
        [JsonIgnore] // senha não será exibida no Swagger ou na resposta
        public required string Password { get; set; }

        public int ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordExpiration { get; set; }
    }
}