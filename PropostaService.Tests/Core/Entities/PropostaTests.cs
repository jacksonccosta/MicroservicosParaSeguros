using PropostaService.Core.Entities;
using Xunit;

namespace PropostaService.Tests.Core.Entities;

public class PropostaTests
{
    [Fact]
    public void Construtor_QuandoChamado_DeveInicializarPropostaCorretamente()
    {
        // Given (Dado)
        var nomeCliente = "Cliente Teste";
        var documentoCliente = "12345678901";
        var valorSeguro = 1500.75m;

        // When (Quando)
        var proposta = new Proposta(nomeCliente, documentoCliente, valorSeguro);

        // Then (Então)
        Assert.NotNull(proposta);
        Assert.Equal(nomeCliente, proposta.NomeCliente);
        Assert.Equal(documentoCliente, proposta.DocumentoCliente);
        Assert.Equal(valorSeguro, proposta.ValorSeguro);
        Assert.Equal(StatusProposta.EmAnalise, proposta.Status);
        Assert.NotEqual(Guid.Empty, proposta.Id);
    }

    [Theory]
    [InlineData(StatusProposta.Aprovada)]
    [InlineData(StatusProposta.Rejeitada)]
    public void AlterarStatus_QuandoChamado_DeveAtualizarOStatusDaProposta(StatusProposta novoStatus)
    {
        // Given (Dado)
        var proposta = new Proposta("Cliente", "123", 100m);

        // When (Quando)
        proposta.AlterarStatus(novoStatus);

        // Then (Então)
        Assert.Equal(novoStatus, proposta.Status);
    }
}