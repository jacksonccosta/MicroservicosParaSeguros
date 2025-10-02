using ContratacaoService.Api.Controllers;
using ContratacaoService.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;
using Xunit.Abstractions;

namespace ContratacaoService.Tests.Integration;

public class ContratacoesControllerTests : IClassFixture<ContratacaoApiFactory>, IAsyncLifetime
{
    private readonly ContratacaoApiFactory _factory;
    private readonly ITestOutputHelper _testOutputHelper;
    private HttpClient _client = null!;
    private WireMockServer _propostaServiceMock = null!;

    private readonly string _testDbConnectionString = "Server=.\\SQL2019;Database=INDT_CONTRATACOES;Trusted_Connection=True;TrustServerCertificate=True;";

    public ContratacoesControllerTests(ContratacaoApiFactory factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// Este método é executado pelo xUnit ANTES de todos os testes nesta classe.
    /// É responsável por configurar o ambiente de teste.
    /// </summary>
    public async Task InitializeAsync()
    {
        _propostaServiceMock = WireMockServer.Start();

        _client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var settings = new Dictionary<string, string?>
                {
                    { "Services:PropostaServiceUrl", _propostaServiceMock.Urls[0] },
                    
                    { "ConnectionStrings:DefaultConnection", _testDbConnectionString }
                };
                config.AddInMemoryCollection(settings);
            });
        }).CreateClient();

        // 3. Prepara o banco de dados de teste
        // Cria uma instância do DbContext usando a mesma connection string
        var options = new DbContextOptionsBuilder<ContratacaoDbContext>()
            .UseSqlServer(_testDbConnectionString)
            .Options;
        var dbContext = new ContratacaoDbContext(options);

        // Garante que as migrations sejam aplicadas e o schema do banco seja criado/atualizado
        await dbContext.Database.MigrateAsync();
    }

    /// <summary>
    /// Este método é executado pelo xUnit DEPOIS de todos os testes nesta classe.
    /// É responsável por limpar o ambiente de teste.
    /// </summary>
    public async Task DisposeAsync()
    {
        // 1. Limpa o banco de dados de teste
        var options = new DbContextOptionsBuilder<ContratacaoDbContext>()
            .UseSqlServer(_testDbConnectionString)
            .Options;
        var dbContext = new ContratacaoDbContext(options);

        // Deleta o banco de dados para garantir que a próxima execução comece do zero
        await dbContext.Database.EnsureDeletedAsync();

        // 2. Para e limpa os recursos do servidor de mock
        _propostaServiceMock.Stop();
        _propostaServiceMock.Dispose();
    }

    [Fact]
    public async Task Contratar_QuandoPropostaEstaAprovada_DeveRetornarOk()
    {
        // Given (Dado que temos uma proposta aprovada no serviço mockado)
        var propostaId = Guid.NewGuid();
        var request = new ContratarPropostaRequest(propostaId);

        _propostaServiceMock
            .Given(Request.Create().WithPath($"/api/propostas/{propostaId}").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(new { id = propostaId, status = "Aprovada" })
            );

        // When (Quando tentamos contratar essa proposta)
        var response = await _client.PostAsJsonAsync("/api/contratacoes", request);

        // Then (Então a operação deve ser bem-sucedida)
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _testOutputHelper.WriteLine($"API retornou um erro inesperado: {errorBody}");
        }

        response.EnsureSuccessStatusCode();

        var contratacao = await response.Content.ReadFromJsonAsync<ContratacaoParaTeste>();
        Assert.NotNull(contratacao);
        Assert.Equal(propostaId, contratacao.PropostaId);
    }

    [Fact]
    public async Task Contratar_QuandoPropostaNaoEstaAprovada_DeveRetornarBadRequest()
    {
        // Given (Dado que temos uma proposta com status Rejeitada)
        var propostaId = Guid.NewGuid();
        var request = new ContratarPropostaRequest(propostaId);

        _propostaServiceMock
            .Given(Request.Create().WithPath($"/api/propostas/{propostaId}").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(new { id = propostaId, status = "Rejeitada" })
            );

        // When (Quando tentamos contratar essa proposta)
        var response = await _client.PostAsJsonAsync("/api/contratacoes", request);

        // Then (Então a API deve retornar um erro de regra de negócio)
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

public class ContratacaoParaTeste
{
    public Guid Id { get; set; }
    public Guid PropostaId { get; set; }
}