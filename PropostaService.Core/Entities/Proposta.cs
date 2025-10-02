namespace PropostaService.Core.Entities;

public enum StatusProposta
{
    EmAnalise,
    Aprovada,
    Rejeitada
}

public class Proposta
{
    public Guid Id { get; private set; }
    public string NomeCliente { get; private set; }
    public string DocumentoCliente { get; private set; }
    public decimal ValorSeguro { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public StatusProposta Status { get; private set; }

    private Proposta() { }

    public Proposta(string nomeCliente, string documentoCliente, decimal valorSeguro)
    {
        Id = Guid.NewGuid();
        NomeCliente = nomeCliente;
        DocumentoCliente = documentoCliente;
        ValorSeguro = valorSeguro;
        DataCriacao = DateTime.UtcNow;
        Status = StatusProposta.EmAnalise;
    }

    public void AlterarStatus(StatusProposta novoStatus)
    {
        Status = novoStatus;
    }
}