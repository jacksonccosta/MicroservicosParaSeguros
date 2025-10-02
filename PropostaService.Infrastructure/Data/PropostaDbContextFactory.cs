using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace PropostaService.Infrastructure.Data;

public class PropostaDbContextFactory : IDesignTimeDbContextFactory<PropostaDbContext>
{
    public PropostaDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../PropostaService.Api"))
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<PropostaDbContext>();

        var connectionString = configuration.GetConnectionString("DB_PROPOSTAS");

        optionsBuilder.UseSqlServer(connectionString);

        return new PropostaDbContext(optionsBuilder.Options);
    }
}