using InstrutorPro.Application.UseCases.Agendamentos.CriarAgendamento;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstrutorPro.API.Controllers;

/// <summary>
/// Controller para gerenciamento de agendamentos de aulas práticas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class AgendamentosController : ControllerBase
{
    private readonly IMediator _mediator;

    public AgendamentosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Cria um novo agendamento de aula prática.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CriarAgendamento(
        [FromBody] CriarAgendamentoCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(ObterAgendamento), new { id }, new { id });
    }

    /// <summary>
    /// Obtém um agendamento pelo ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterAgendamento(Guid id, CancellationToken cancellationToken)
    {
        // TODO: Implementar query GetAgendamentoById
        return Ok(new { id, message = "TODO: Implementar GetAgendamentoByIdQuery" });
    }

    /// <summary>
    /// Confirma um agendamento pendente.
    /// </summary>
    [HttpPatch("{id:guid}/confirmar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmarAgendamento(Guid id, CancellationToken cancellationToken)
    {
        // TODO: Implementar ConfirmarAgendamentoCommand
        return NoContent();
    }

    /// <summary>
    /// Cancela um agendamento.
    /// </summary>
    [HttpPatch("{id:guid}/cancelar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelarAgendamento(Guid id, CancellationToken cancellationToken)
    {
        // TODO: Implementar CancelarAgendamentoCommand
        return NoContent();
    }
}
