using Moq;
using ContratacaoService.Core.Entities;
using ContratacaoService.Core.Ports;
using ContratacaoService.Core.Services;
using Xunit;

namespace ContratacaoService.Tests.Core.Services;

public class ContratacaoUseCaseServiceTests
{
    private readonly Mock<IContratacaoRepository> _mockContratacaoRepository;
    private readonly Mock<IPropostaGateway> _mockPropostaGateway;
    private readonly ContratacaoUseCaseService _service;

    public ContratacaoUseCaseServiceTests()
    {
        _mockContratacaoRepository = new Mock<IContratacaoRepository>();
        _mockPropostaGateway = new Mock<IPropostaGateway>();
        _service = new ContratacaoUseCaseService(_mockContratacaoRepository.Object, _mockPropostaGateway.Object);
    }

    [Fact]
    public async Task ContratarPropostaAsync_QuandoPropostaEstaAprovada_DeveCriarContratacao()
    {
        // Given (Dado)
        var propostaId = Guid.NewGuid();
        var propostaStatusDto = new PropostaStatusDto(propostaId, "Aprovada");

        _mockPropostaGateway.Setup(gateway => gateway.ObterStatusPropostaAsync(propostaId)).ReturnsAsync(propostaStatusDto);

        // When (Quando)
        var contratacao = await _service.ContratarPropostaAsync(propostaId);

        // Then (Então)
        Assert.NotNull(contratacao);
        Assert.Equal(propostaId, contratacao.PropostaId);
        _mockContratacaoRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<Contratacao>()), Times.Once);
    }

    [Theory]
    [InlineData("Rejeitada")]
    [InlineData("EmAnalise")]
    public async Task ContratarPropostaAsync_QuandoPropostaNaoEstaAprovada_DeveLancarInvalidOperationException(string status)
    {
        // Given (Dado)
        var propostaId = Guid.NewGuid();
        var propostaStatusDto = new PropostaStatusDto(propostaId, status);
        _mockPropostaGateway.Setup(gateway => gateway.ObterStatusPropostaAsync(propostaId)).ReturnsAsync(propostaStatusDto);

        // When (Quando) & Then (Então)
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ContratarPropostaAsync(propostaId));
        Assert.Equal("Apenas propostas com status 'Aprovada' podem ser contratadas.", exception.Message);
        _mockContratacaoRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<Contratacao>()), Times.Never);
    }

    [Fact]
    public async Task ContratarPropostaAsync_QuandoPropostaNaoExiste_DeveLancarInvalidOperationException()
    {
        // Given (Dado)
        var propostaId = Guid.NewGuid();
        _mockPropostaGateway.Setup(gateway => gateway.ObterStatusPropostaAsync(propostaId)).ReturnsAsync((PropostaStatusDto?)null);

        // When (Quando) & Then (Então)
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ContratarPropostaAsync(propostaId));
        Assert.Equal("Proposta não encontrada.", exception.Message);
    }
}