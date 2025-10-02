using ContratacaoService.Core.Entities;

namespace ContratacaoService.Core.Ports;

public interface IContratacaoRepository
{
    Task AdicionarAsync(Contratacao contratacao);
}