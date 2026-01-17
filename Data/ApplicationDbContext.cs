using Microsoft.EntityFrameworkCore;
using PokemonTcgApi.Models;

namespace PokemonTcgApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Classic DbContext que usa DbContextOptions para configurar a conexão com o banco de dados
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {}
        public DbSet<PriceHistory> PriceHistories { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder){

            // Configure PriceHistory entity
            modelBuilder.Entity<PriceHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Source).IsRequired().HasMaxLength(50);
            });
        }

    }
}