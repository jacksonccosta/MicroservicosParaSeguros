using Microsoft.AspNetCore.Mvc;
using ContratacaoService.Core.Entities;
using ContratacaoService.Core.Services;

namespace ContratacaoService.Api.Controllers;

/// <summary>
/// DTO para solicitar a contratação de uma proposta.
/// </summary>
/// <param name="PropostaId">O ID (GUID) da proposta que deve ser contratada.</param>
public record ContratarPropostaRequest(Guid PropostaId);

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ContratacoesController : ControllerBase
{
    private readonly ContratacaoUseCaseService _contratacaoService;

    public ContratacoesController(ContratacaoUseCaseService contratacaoService) => _contratacaoService = contratacaoService;

    /// <summary>
    /// Efetiva a contratação de uma proposta de seguro.
    /// </summary>
    /// <remarks>
    /// **Regra de Negócio:** Este endpoint se comunica com o PropostaService para validar
    /// se a proposta existe e se seu status é **"Aprovada"**. Somente nessas condições
    /// a contratação é registrada.
    /// </remarks>
    /// <param name="request">Dados da contratação.</param>
    /// <returns>Os dados da contratação recém-criada.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Contratacao), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Contratar([FromBody] ContratarPropostaRequest request)
    {
        try
        {
            var contratacao = await _contratacaoService.ContratarPropostaAsync(request.PropostaId);
            return Ok(contratacao);
        }
        catch (InvalidOperationException ex)
        {
            // Retorna um BadRequest com a mensagem de erro da regra de negócio
            return BadRequest(new { Error = ex.Message });
        }
    }
}