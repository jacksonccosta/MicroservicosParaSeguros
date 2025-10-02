using ContratacaoService.Core.Entities;
using ContratacaoService.Core.Ports;

namespace ContratacaoService.Core.Services;

public class ContratacaoUseCaseService(IContratacaoRepository contratacaoRepository, IPropostaGateway propostaGateway)
{
    private readonly IContratacaoRepository _contratacaoRepository = contratacaoRepository;
    private readonly IPropostaGateway _propostaGateway = propostaGateway;

    public async Task<Contratacao?> ContratarPropostaAsync(Guid propostaId)
    {
        var statusProposta = await _propostaGateway.ObterStatusPropostaAsync(propostaId);

        if (statusProposta is null)
            throw new InvalidOperationException("Proposta não encontrada.");

        if (statusProposta.Status != "Aprovada")
            throw new InvalidOperationException("Apenas propostas com status 'Aprovada' podem ser contratadas.");

        var contratacao = new Contratacao(propostaId);
        await _contratacaoRepository.AdicionarAsync(contratacao);

        return contratacao;
    }
}