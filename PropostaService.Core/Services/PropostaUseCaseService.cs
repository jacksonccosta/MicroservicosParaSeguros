using PropostaService.Core.Entities;
using PropostaService.Core.Ports;

namespace PropostaService.Core.Services;

public class PropostaUseCaseService(IPropostaRepository propostaRepository)
{
    private readonly IPropostaRepository _propostaRepository = propostaRepository;

    public async Task<Proposta> CriarPropostaAsync(string nome, string documento, decimal valor)
    {
        var proposta = new Proposta(nome, documento, valor);
        await _propostaRepository.AdicionarAsync(proposta);
        return proposta;
    }

    public async Task<Proposta?> AlterarStatusAsync(Guid id, StatusProposta novoStatus)
    {
        var proposta = await _propostaRepository.ObterPorIdAsync(id);
        if (proposta == null)
        {
            return null;
        }

        proposta.AlterarStatus(novoStatus);
        await _propostaRepository.AtualizarAsync(proposta);
        return proposta;
    }

    public Task<IEnumerable<Proposta>> ListarPropostasAsync() => _propostaRepository.ListarAsync();
    public Task<Proposta?> ObterPropostaPorIdAsync(Guid id) => _propostaRepository.ObterPorIdAsync(id);
}