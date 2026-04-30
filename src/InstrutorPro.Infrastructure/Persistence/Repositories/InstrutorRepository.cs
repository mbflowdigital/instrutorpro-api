using InstrutorPro.Application.Interfaces.Repositories;
using InstrutorPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InstrutorPro.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação do repositório de instrutores usando Entity Framework Core com PostgreSQL.
/// </summary>
public class InstrutorRepository : IInstrutorRepository
{
    private readonly AppDbContext _context;

    public InstrutorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Instrutor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Instrutores
            .Include(i => i.Credenciamento)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<Instrutor?> GetByCPFAsync(string cpf, CancellationToken cancellationToken = default) =>
        await _context.Instrutores.FirstOrDefaultAsync(i => i.CPF == cpf, cancellationToken);

    public async Task<Instrutor?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _context.Instrutores.FirstOrDefaultAsync(i => i.Email == email, cancellationToken);

    public async Task<IEnumerable<Instrutor>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Instrutores.ToListAsync(cancellationToken);

    public async Task<IEnumerable<Instrutor>> GetAtivosAsync(CancellationToken cancellationToken = default) =>
        await _context.Instrutores
            .Where(i => i.Status == Domain.Enums.StatusInstrutor.Ativo)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Instrutor instrutor, CancellationToken cancellationToken = default)
    {
        await _context.Instrutores.AddAsync(instrutor, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Instrutor instrutor, CancellationToken cancellationToken = default)
    {
        _context.Instrutores.Update(instrutor);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Instrutores.AnyAsync(i => i.Id == id, cancellationToken);
}
