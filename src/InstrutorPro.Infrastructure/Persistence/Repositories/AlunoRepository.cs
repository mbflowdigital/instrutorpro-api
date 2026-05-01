using InstrutorPro.Application.Interfaces.Repositories;
using InstrutorPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InstrutorPro.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação do repositório de alunos usando Entity Framework Core com PostgreSQL.
/// </summary>
public class AlunoRepository : IAlunoRepository
{
    private readonly AppDbContext _context;

    public AlunoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Aluno?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Alunos.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<Aluno?> GetByCPFAsync(string cpf, CancellationToken cancellationToken = default) =>
        await _context.Alunos.FirstOrDefaultAsync(a => a.CPF == cpf, cancellationToken);

    public async Task<Aluno?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _context.Alunos.FirstOrDefaultAsync(a => a.Email == email, cancellationToken);

    public async Task<IEnumerable<Aluno>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Alunos.ToListAsync(cancellationToken);

    public async Task AddAsync(Aluno aluno, CancellationToken cancellationToken = default)
    {
        await _context.Alunos.AddAsync(aluno, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Aluno aluno, CancellationToken cancellationToken = default)
    {
        _context.Alunos.Update(aluno);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Alunos.AnyAsync(a => a.Id == id, cancellationToken);
}
