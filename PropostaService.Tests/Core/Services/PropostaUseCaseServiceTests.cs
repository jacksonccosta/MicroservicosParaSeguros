using Moq;
using PropostaService.Core.Entities;
using PropostaService.Core.Ports;
using PropostaService.Core.Services;
using Xunit;

namespace PropostaService.Tests.Core.Services;

public class PropostaUseCaseServiceTests
{
    private readonly Mock<IPropostaRepository> _mockPropostaRepository;
    private readonly PropostaUseCaseService _service;

    public PropostaUseCaseServiceTests()
    {
        _mockPropostaRepository = new Mock<IPropostaRepository>();
        _service = new PropostaUseCaseService(_mockPropostaRepository.Object);
    }

    [Fact]
    public async Task CriarPropostaAsync_QuandoDadosValidos_DeveChamarRepositorioEretornarProposta()
    {
        // Given (Dado)
        var nome = "Cliente Valido";
        var documento = "123.456.789-00";
        var valor = 2000m;

        // When (Quando)
        var propostaCriada = await _service.CriarPropostaAsync(nome, documento, valor);

        // Then (Então)
        Assert.NotNull(propostaCriada);
        Assert.Equal(nome, propostaCriada.NomeCliente);
        // Verifica se o método AdicionarAsync do repositório foi chamado exatamente uma vez
        _mockPropostaRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<Proposta>()), Times.Once);
    }

    [Fact]
    public async Task AlterarStatusAsync_QuandoPropostaExiste_DeveAlterarStatusEChamarRepositorio()
    {
        // Given (Dado)
        var propostaId = Guid.NewGuid();
        var novoStatus = StatusProposta.Aprovada;
        var propostaExistente = new Proposta("Cliente Existente", "987654321", 500m);

        _mockPropostaRepository.Setup(repo => repo.ObterPorIdAsync(propostaId)).ReturnsAsync(propostaExistente);

        // When (Quando)
        var propostaAtualizada = await _service.AlterarStatusAsync(propostaId, novoStatus);

        // Then (Então)
        Assert.NotNull(propostaAtualizada);
        Assert.Equal(novoStatus, propostaAtualizada.Status);
        _mockPropostaRepository.Verify(repo => repo.AtualizarAsync(propostaExistente), Times.Once);
    }

    [Fact]
    public async Task AlterarStatusAsync_QuandoPropostaNaoExiste_DeveRetornarNull()
    {
        // Given (Dado)
        var propostaId = Guid.NewGuid();
        _mockPropostaRepository.Setup(repo => repo.ObterPorIdAsync(propostaId)).ReturnsAsync((Proposta?)null);

        // When (Quando)
        var resultado = await _service.AlterarStatusAsync(propostaId, StatusProposta.Aprovada);

        // Then (Então)
        Assert.Null(resultado);
        _mockPropostaRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<Proposta>()), Times.Never);
    }
}