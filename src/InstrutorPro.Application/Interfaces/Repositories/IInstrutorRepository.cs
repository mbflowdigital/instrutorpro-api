using InstrutorPro.Domain.Entities;

namespace InstrutorPro.Application.Interfaces.Repositories;

/// <summary>
/// Interface do repositório de instrutores.
/// Define as operações de persistência necessárias para o domínio.
/// </summary>
public interface IInstrutorRepository
{
    Task<Instrutor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Instrutor?> GetByCPFAsync(string cpf, CancellationToken cancellationToken = default);
    Task<Instrutor?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Instrutor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Instrutor>> GetAtivosAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Instrutor instrutor, CancellationToken cancellationToken = default);
    Task UpdateAsync(Instrutor instrutor, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
