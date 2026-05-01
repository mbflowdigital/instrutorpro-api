using InstrutorPro.Domain.Entities;

namespace InstrutorPro.Application.Interfaces.Repositories;

/// <summary>
/// Interface do repositório de agendamentos.
/// </summary>
public interface IAgendamentoRepository
{
    Task<Agendamento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> GetByInstrutorIdAsync(Guid instrutorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> GetByAlunoIdAsync(Guid alunoId, CancellationToken cancellationToken = default);
    Task AddAsync(Agendamento agendamento, CancellationToken cancellationToken = default);
    Task UpdateAsync(Agendamento agendamento, CancellationToken cancellationToken = default);
    Task<bool> ExistsConflictAsync(Guid instrutorId, DateTime dataHora, int duracaoHoras, CancellationToken cancellationToken = default);
}
