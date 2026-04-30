using InstrutorPro.Application.UseCases.Alunos.CadastrarAluno;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstrutorPro.API.Controllers;

/// <summary>
/// Controller para gerenciamento de alunos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AlunosController : ControllerBase
{
    private readonly IMediator _mediator;

    public AlunosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Cadastra um novo aluno na plataforma.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CadastrarAluno(
        [FromBody] CadastrarAlunoCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(ObterAluno), new { id }, new { id });
    }

    /// <summary>
    /// Obtém um aluno pelo ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterAluno(Guid id, CancellationToken cancellationToken)
    {
        // TODO: Implementar query GetAlunoById
        return Ok(new { id, message = "TODO: Implementar GetAlunoByIdQuery" });
    }
}
