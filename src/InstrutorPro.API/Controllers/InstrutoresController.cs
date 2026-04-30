using InstrutorPro.Application.UseCases.Instrutores.CadastrarInstrutor;
using InstrutorPro.Application.UseCases.Instrutores.ValidarCredenciamento;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstrutorPro.API.Controllers;

/// <summary>
/// Controller para gerenciamento de instrutores de trânsito.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class InstrutoresController : ControllerBase
{
    private readonly IMediator _mediator;

    public InstrutoresController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Cadastra um novo instrutor na plataforma.
    /// O instrutor fica com status Pendente até validar o credenciamento.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CadastrarInstrutor(
        [FromBody] CadastrarInstrutorCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(ObterInstrutor), new { id }, new { id });
    }

    /// <summary>
    /// Obtém um instrutor pelo ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterInstrutor(Guid id, CancellationToken cancellationToken)
    {
        // TODO: Implementar query GetInstrutorById
        return Ok(new { id, message = "TODO: Implementar GetInstrutorByIdQuery" });
    }

    /// <summary>
    /// Lista todos os instrutores ativos disponíveis para contratação.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarInstrutoresAtivos(CancellationToken cancellationToken)
    {
        // TODO: Implementar query ListInstrutoresAtivosQuery com filtros (estado, categoria, etc.)
        return Ok(new { message = "TODO: Implementar ListInstrutoresAtivosQuery" });
    }

    /// <summary>
    /// Valida o credenciamento do instrutor junto ao Detran/Senatran.
    /// Atualiza o status do instrutor para Ativo ou Suspenso conforme resultado.
    /// </summary>
    [HttpPost("{id:guid}/validar-credenciamento")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ValidarCredenciamento(
        Guid id,
        [FromBody] ValidarCredenciamentoRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ValidarCredenciamentoCommand(id, request.NumeroCredencial);
        var valido = await _mediator.Send(command, cancellationToken);

        return Ok(new { valido, mensagem = valido ? "Credenciamento válido. Instrutor ativado." : "Credenciamento inválido ou não encontrado." });
    }
}

/// <summary>
/// Request body para validação de credenciamento.
/// </summary>
public record ValidarCredenciamentoRequest(string NumeroCredencial);
