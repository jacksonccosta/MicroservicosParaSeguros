using Microsoft.EntityFrameworkCore;
using PropostaService.Core.Entities;
using PropostaService.Core.Ports;
using PropostaService.Infrastructure.Data;

namespace PropostaService.Infrastructure.Adapters;

public class PropostaRepository(PropostaDbContext context) : IPropostaRepository
{
    private readonly PropostaDbContext _context = context;

    public async Task AdicionarAsync(Proposta proposta)
    {
        await _context.Propostas.AddAsync(proposta);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Proposta proposta)
    {
        _context.Propostas.Update(proposta);
        await _context.SaveChangesAsync();
    }

    public Task<Proposta?> ObterPorIdAsync(Guid id) => _context.Propostas.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Proposta>> ListarAsync() => await _context.Propostas.ToListAsync();
}