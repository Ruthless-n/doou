using System.ComponentModel.DataAnnotations;

namespace Doou.Api.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public ICollection<Donation>? Donations { get; set; }

        public static readonly Category Vestuario = new() { Id = 1, Name = "Vestuário" };
        public static readonly Category ArtigosCasa = new() { Id = 2, Name = "Artigos para casa" };
        public static readonly Category ParaBebes = new() { Id = 3, Name = "Para bebês e crianças" };
        public static readonly Category MoveisEletros = new() { Id = 4, Name = "Móveis e eletros" };
        public static readonly Category Higiene = new() { Id = 5, Name = "Higiene" };
        public static readonly Category Educacional = new() { Id = 6, Name = "Educacional" };
        public static readonly Category Eletronicos = new() { Id = 7, Name = "Eletrônicos" };
        public static readonly Category Outros = new() { Id = 8, Name = "Outros" };
    }
}