using System.Net.Http;
using System.Text.Json;
using ContratacaoService.Core.Ports;
using Microsoft.Extensions.Configuration;

namespace ContratacaoService.Infrastructure.Adapters;

public class PropostaHttpGateway : IPropostaGateway
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public PropostaHttpGateway(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<PropostaStatusDto?> ObterStatusPropostaAsync(Guid propostaId)
    {
        var client = _httpClientFactory.CreateClient();
        var propostaServiceUrl = _configuration["Services:PropostaServiceUrl"];

        var response = await client.GetAsync($"{propostaServiceUrl}/api/propostas/{propostaId}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var proposta = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var status = proposta.GetProperty("status").GetString();
        return new PropostaStatusDto(propostaId, status!);
    }
}