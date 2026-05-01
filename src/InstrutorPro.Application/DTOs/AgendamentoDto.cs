using InstrutorPro.Domain.Enums;

namespace InstrutorPro.Application.DTOs;

/// <summary>
/// DTO de retorno com os dados de um agendamento.
/// </summary>
public record AgendamentoDto(
    Guid Id,
    Guid InstrutorId,
    string NomeInstrutor,
    Guid AlunoId,
    string NomeAluno,
    DateTime DataHora,
    int DuracaoHoras,
    decimal Valor,
    StatusAgendamento Status,
    DateTime CreatedAt
);
