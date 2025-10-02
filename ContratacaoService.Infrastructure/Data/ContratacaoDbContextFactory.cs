using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ContratacaoService.Infrastructure.Data;

public class ContratacaoDbContextFactory : IDesignTimeDbContextFactory<ContratacaoDbContext>
{
    public ContratacaoDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../ContratacaoService.Api"))
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ContratacaoDbContext>();

        var connectionString = configuration.GetConnectionString("DB_CONTRATACOES");

        optionsBuilder.UseSqlServer(connectionString);

        return new ContratacaoDbContext(optionsBuilder.Options);
    }
}