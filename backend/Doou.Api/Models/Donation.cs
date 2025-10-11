using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doou.Api.Models
{
    public class Donation
    {
        [Key]
        public int DonationId { get; set; }
        public string? Name { get; set; }
        [MaxLength(11)]
        public string? CPF { get; set; }
        public string? Email { get; set; }
        public DateTime BirthDate { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public string? Address { get; set; }
    }
}