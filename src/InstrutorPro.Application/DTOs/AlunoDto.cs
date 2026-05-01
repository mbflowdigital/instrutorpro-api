namespace InstrutorPro.Application.DTOs;

/// <summary>
/// DTO de retorno com os dados de um aluno.
/// </summary>
public record AlunoDto(
    Guid Id,
    string Nome,
    string Email,
    string Telefone,
    string? CidadeId,
    string Estado,
    DateTime CreatedAt
);
