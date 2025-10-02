using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PropostaService.Api;
using PropostaService.Api.Controllers;
using PropostaService.Infrastructure.Data;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PropostaService.Tests.Integration;

public class PropostasControllerTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _testOutputHelper;
    private HttpClient _client = null!;

    private readonly string _testDbConnectionString = "Server=.\\SQL2019;Database=INDT_PROPOSTAS;Trusted_Connection=True;TrustServerCertificate=True;";

    public PropostasControllerTests(WebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
    }

    public async Task InitializeAsync()
    {
        _client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var settings = new Dictionary<string, string?>
                {
                    { "ConnectionStrings:DefaultConnection", _testDbConnectionString }
                };
                config.AddInMemoryCollection(settings);
            });
        }).CreateClient();

        var options = new DbContextOptionsBuilder<PropostaDbContext>()
            .UseSqlServer(_testDbConnectionString)
            .Options;
        var dbContext = new PropostaDbContext(options);

        await dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        var options = new DbContextOptionsBuilder<PropostaDbContext>()
            .UseSqlServer(_testDbConnectionString)
            .Options;
        var dbContext = new PropostaDbContext(options);

        await dbContext.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task Criar_QuandoPropostaValida_DeveRetornarCreatedEPropostaComId()
    {
        // Given (Dado)
        var novaPropostaRequest = new CriarPropostaRequest(
            "Cliente de Integração",
            "99988877700",
            5000m
        );

        // When (Quando)
        var response = await _client.PostAsJsonAsync("/api/propostas", novaPropostaRequest);

        // Then (Então)
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _testOutputHelper.WriteLine($"API retornou um erro inesperado: {errorBody}");
        }

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var propostaCriada = await response.Content.ReadFromJsonAsync<PropostaParaTeste>();
        Assert.NotNull(propostaCriada);
        Assert.NotEqual(Guid.Empty, propostaCriada.Id);
        Assert.Equal("Cliente de Integração", propostaCriada.NomeCliente);
        Assert.Equal("EmAnalise", propostaCriada.Status);
    }
}

public class PropostaParaTeste
{
    public Guid Id { get; set; }
    public string NomeCliente { get; set; }
    public string Status { get; set; }
}