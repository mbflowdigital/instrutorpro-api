using InstrutorPro.Domain.Entities;

namespace InstrutorPro.Application.Interfaces.Repositories;

/// <summary>
/// Interface do repositório de alunos.
/// </summary>
public interface IAlunoRepository
{
    Task<Aluno?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Aluno?> GetByCPFAsync(string cpf, CancellationToken cancellationToken = default);
    Task<Aluno?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Aluno>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Aluno aluno, CancellationToken cancellationToken = default);
    Task UpdateAsync(Aluno aluno, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
