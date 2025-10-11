using Doou.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Doou.Api.Config
{
    public class DoouDbContext : DbContext
    {
        public DoouDbContext(DbContextOptions<DoouDbContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                Category.ArtigosCasa,
                Category.Vestuario,
                Category.ParaBebes,
                Category.MoveisEletros,
                Category.Higiene,
                Category.Educacional,
                Category.Eletronicos,
                Category.Outros
            );
        }
    }
}