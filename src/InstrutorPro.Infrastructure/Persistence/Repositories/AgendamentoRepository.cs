using InstrutorPro.Application.Interfaces.Repositories;
using InstrutorPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InstrutorPro.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação do repositório de agendamentos usando Entity Framework Core com PostgreSQL.
/// </summary>
public class AgendamentoRepository : IAgendamentoRepository
{
    private readonly AppDbContext _context;

    public AgendamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Agendamento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Agendamentos
            .Include(a => a.Instrutor)
            .Include(a => a.Aluno)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IEnumerable<Agendamento>> GetByInstrutorIdAsync(Guid instrutorId, CancellationToken cancellationToken = default) =>
        await _context.Agendamentos
            .Include(a => a.Aluno)
            .Where(a => a.InstrutorId == instrutorId)
            .OrderByDescending(a => a.DataHora)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Agendamento>> GetByAlunoIdAsync(Guid alunoId, CancellationToken cancellationToken = default) =>
        await _context.Agendamentos
            .Include(a => a.Instrutor)
            .Where(a => a.AlunoId == alunoId)
            .OrderByDescending(a => a.DataHora)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Agendamento agendamento, CancellationToken cancellationToken = default)
    {
        await _context.Agendamentos.AddAsync(agendamento, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Agendamento agendamento, CancellationToken cancellationToken = default)
    {
        _context.Agendamentos.Update(agendamento);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Verifica se o instrutor possui agendamento conflitante no período informado.
    /// </summary>
    public async Task<bool> ExistsConflictAsync(
        Guid instrutorId,
        DateTime dataHora,
        int duracaoHoras,
        CancellationToken cancellationToken = default)
    {
        var dataFim = dataHora.AddHours(duracaoHoras);

        return await _context.Agendamentos.AnyAsync(a =>
            a.InstrutorId == instrutorId &&
            a.Status != Domain.Enums.StatusAgendamento.Cancelado &&
            a.DataHora < dataFim &&
            a.DataHora.AddHours(a.DuracaoHoras) > dataHora,
            cancellationToken);
    }
}
