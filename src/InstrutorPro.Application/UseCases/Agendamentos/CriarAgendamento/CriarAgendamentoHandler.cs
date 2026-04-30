using InstrutorPro.Application.Interfaces.Repositories;
using InstrutorPro.Domain.Entities;
using InstrutorPro.Domain.Enums;
using MediatR;

namespace InstrutorPro.Application.UseCases.Agendamentos.CriarAgendamento;

/// <summary>
/// Handler para criação de agendamentos de aulas práticas.
/// Valida disponibilidade do instrutor, status ativo e calcula o valor com base na tarifa por hora.
/// </summary>
public class CriarAgendamentoHandler : IRequestHandler<CriarAgendamentoCommand, Guid>
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IInstrutorRepository _instrutorRepository;
    private readonly IAlunoRepository _alunoRepository;

    public CriarAgendamentoHandler(
        IAgendamentoRepository agendamentoRepository,
        IInstrutorRepository instrutorRepository,
        IAlunoRepository alunoRepository)
    {
        _agendamentoRepository = agendamentoRepository;
        _instrutorRepository = instrutorRepository;
        _alunoRepository = alunoRepository;
    }

    public async Task<Guid> Handle(CriarAgendamentoCommand request, CancellationToken cancellationToken)
    {
        var instrutor = await _instrutorRepository.GetByIdAsync(request.InstrutorId, cancellationToken)
            ?? throw new KeyNotFoundException($"Instrutor com ID {request.InstrutorId} não encontrado.");

        if (instrutor.Status != StatusInstrutor.Ativo)
            throw new InvalidOperationException("O instrutor não está ativo na plataforma.");

        var alunoExiste = await _alunoRepository.ExistsAsync(request.AlunoId, cancellationToken);
        if (!alunoExiste)
            throw new KeyNotFoundException($"Aluno com ID {request.AlunoId} não encontrado.");

        // Verifica conflito de horário para o instrutor
        var temConflito = await _agendamentoRepository.ExistsConflictAsync(
            request.InstrutorId,
            request.DataHora,
            request.DuracaoHoras,
            cancellationToken);

        if (temConflito)
            throw new InvalidOperationException("O instrutor já possui agendamento neste horário.");

        // Calcula o valor total baseado no valor por hora do instrutor
        var valor = instrutor.ValorHoraAula * request.DuracaoHoras;

        var agendamento = new Agendamento(
            instrutorId: request.InstrutorId,
            alunoId: request.AlunoId,
            dataHora: request.DataHora,
            duracaoHoras: request.DuracaoHoras,
            valor: valor
        );

        await _agendamentoRepository.AddAsync(agendamento, cancellationToken);

        return agendamento.Id;
    }
}
