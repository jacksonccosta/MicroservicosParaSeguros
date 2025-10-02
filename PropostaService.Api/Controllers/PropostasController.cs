using Microsoft.AspNetCore.Mvc;
using PropostaService.Core.Entities;
using PropostaService.Core.Services;

namespace PropostaService.Api.Controllers;

/// <summary>
/// DTO para a criação de uma nova proposta.
/// </summary>
/// <param name="NomeCliente">INDT</param>
/// <param name="DocumentoCliente">...</param>
/// <param name="ValorSeguro">Valor monetário do seguro proposto.</param>
public record CriarPropostaRequest(string NomeCliente, string DocumentoCliente, decimal ValorSeguro);

/// <summary>
/// DTO para a alteração de status de uma proposta.
/// </summary>
/// <param name="NovoStatus">O novo status da proposta. Valores possíveis: EmAnalise, Aprovada, Rejeitada.</param>
public record AlterarStatusRequest(StatusProposta NovoStatus);

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PropostasController : ControllerBase
{
    private readonly PropostaUseCaseService _propostaService;

    public PropostasController(PropostaUseCaseService propostaService) => _propostaService = propostaService;

    /// <summary>
    /// Cria uma nova proposta de seguro.
    /// </summary>
    /// <remarks>
    /// A proposta é criada com o status inicial "EmAnalise".
    /// </remarks>
    /// <param name="request">Dados da proposta a ser criada.</param>
    /// <returns>A proposta recém-criada.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Proposta), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarPropostaRequest request)
    {
        var proposta = await _propostaService.CriarPropostaAsync(request.NomeCliente, request.DocumentoCliente, request.ValorSeguro);
        return CreatedAtAction(nameof(ObterPorId), new { id = proposta.Id }, proposta);
    }

    /// <summary>
    /// Lista todas as propostas cadastradas.
    /// </summary>
    /// <returns>Uma lista de propostas.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Proposta>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar() => Ok(await _propostaService.ListarPropostasAsync());

    /// <summary>
    /// Busca uma proposta específica pelo seu ID.
    /// </summary>
    /// <param name="id">O ID (GUID) da proposta.</param>
    /// <returns>Os detalhes da proposta encontrada.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Proposta), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var proposta = await _propostaService.ObterPropostaPorIdAsync(id);
        return proposta is null ? NotFound() : Ok(proposta);
    }

    /// <summary>
    /// Altera o status de uma proposta existente.
    /// </summary>
    /// <param name="id">O ID (GUID) da proposta a ser alterada.</param>
    /// <param name="request">O novo status para a proposta.</param>
    /// <returns>A proposta com o status atualizado.</returns>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(Proposta), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlterarStatus(Guid id, [FromBody] AlterarStatusRequest request)
    {
        var proposta = await _propostaService.AlterarStatusAsync(id, request.NovoStatus);
        return proposta is null ? NotFound() : Ok(proposta);
    }
}