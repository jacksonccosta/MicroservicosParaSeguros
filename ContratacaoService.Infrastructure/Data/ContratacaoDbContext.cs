using ContratacaoService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ContratacaoService.Infrastructure.Data;

public class ContratacaoDbContext(DbContextOptions<ContratacaoDbContext> options) : DbContext(options)
{
    public DbSet<Contratacao> Contratacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contratacao>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.PropostaId).IsRequired();
            entity.HasIndex(e => e.PropostaId).IsUnique();
        });
    }
}