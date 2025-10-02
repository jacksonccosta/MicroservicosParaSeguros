namespace ContratacaoService.Core.Ports;
public record PropostaStatusDto(Guid Id, string Status);

public interface IPropostaGateway
{
    Task<PropostaStatusDto?> ObterStatusPropostaAsync(Guid propostaId);
}