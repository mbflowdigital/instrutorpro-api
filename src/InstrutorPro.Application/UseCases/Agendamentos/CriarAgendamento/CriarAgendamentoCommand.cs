using MediatR;

namespace InstrutorPro.Application.UseCases.Agendamentos.CriarAgendamento;

/// <summary>
/// Command para criar um novo agendamento de aula prática.
/// </summary>
public record CriarAgendamentoCommand(
    Guid InstrutorId,
    Guid AlunoId,
    DateTime DataHora,
    int DuracaoHoras
) : IRequest<Guid>;
