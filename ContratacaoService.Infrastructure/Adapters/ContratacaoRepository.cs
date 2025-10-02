using ContratacaoService.Core.Entities;
using ContratacaoService.Core.Ports;
using ContratacaoService.Infrastructure.Data;
using System.Threading.Tasks;

namespace ContratacaoService.Infrastructure.Adapters;

public class ContratacaoRepository(ContratacaoDbContext context) : IContratacaoRepository
{
    private readonly ContratacaoDbContext _context = context;

    /// <summary>
    /// Adiciona uma nova contratação ao banco de dados.
    /// </summary>
    /// <param name="contratacao">A entidade de Contratação a ser salva.</param>
    public async Task AdicionarAsync(Contratacao contratacao)
    {
        await _context.Contratacoes.AddAsync(contratacao);

        await _context.SaveChangesAsync();
    }
}