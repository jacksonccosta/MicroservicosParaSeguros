using PropostaService.Core.Entities;

namespace PropostaService.Core.Ports;

public interface IPropostaRepository
{
    Task AdicionarAsync(Proposta proposta);
    Task<Proposta?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Proposta>> ListarAsync();
    Task AtualizarAsync(Proposta proposta);
}