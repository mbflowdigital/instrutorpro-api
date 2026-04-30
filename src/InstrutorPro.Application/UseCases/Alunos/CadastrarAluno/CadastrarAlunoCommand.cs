using MediatR;

namespace InstrutorPro.Application.UseCases.Alunos.CadastrarAluno;

/// <summary>
/// Command para cadastrar um novo aluno na plataforma.
/// </summary>
public record CadastrarAlunoCommand(
    string Nome,
    string CPF,
    string Email,
    string Telefone,
    string Estado,
    string? CidadeId = null
) : IRequest<Guid>;
