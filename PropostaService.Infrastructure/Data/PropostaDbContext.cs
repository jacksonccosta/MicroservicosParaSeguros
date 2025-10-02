using Microsoft.EntityFrameworkCore;
using PropostaService.Core.Entities;

namespace PropostaService.Infrastructure.Data;

public class PropostaDbContext(DbContextOptions<PropostaDbContext> options) : DbContext(options)
{
    public DbSet<Proposta> Propostas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Proposta>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ValorSeguro).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).HasConversion<string>();
        });
    }
}