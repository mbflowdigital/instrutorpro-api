using InstrutorPro.Domain.Enums;

namespace InstrutorPro.Domain.Entities;

/// <summary>
/// Entidade que representa um agendamento de aula prática entre aluno e instrutor.
/// Contém as regras de transição de status do agendamento.
/// </summary>
public class Agendamento
{
    public Guid Id { get; private set; }
    public Guid InstrutorId { get; private set; }
    public Guid AlunoId { get; private set; }
    public DateTime DataHora { get; private set; }
    public int DuracaoHoras { get; private set; }
    public decimal Valor { get; private set; }
    public StatusAgendamento Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navegação
    public Instrutor? Instrutor { get; private set; }
    public Aluno? Aluno { get; private set; }
    public Avaliacao? Avaliacao { get; private set; }

    private Agendamento() { }

    public Agendamento(
        Guid instrutorId,
        Guid alunoId,
        DateTime dataHora,
        int duracaoHoras,
        decimal valor)
    {
        if (dataHora < DateTime.UtcNow.AddMinutes(5))
            throw new ArgumentException("O agendamento deve ser com pelo menos 5 minutos de antecedência.", nameof(dataHora));
        if (duracaoHoras < 1)
            throw new ArgumentException("A duração mínima é de 1 hora.", nameof(duracaoHoras));
        if (valor <= 0)
            throw new ArgumentException("O valor deve ser maior que zero.", nameof(valor));

        Id = Guid.NewGuid();
        InstrutorId = instrutorId;
        AlunoId = alunoId;
        DataHora = dataHora;
        DuracaoHoras = duracaoHoras;
        Valor = valor;
        Status = StatusAgendamento.Pendente;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Confirma o agendamento após aceite do instrutor.
    /// </summary>
    public void Confirmar()
    {
        if (Status != StatusAgendamento.Pendente)
            throw new InvalidOperationException("Apenas agendamentos pendentes podem ser confirmados.");

        Status = StatusAgendamento.Confirmado;
    }

    /// <summary>
    /// Cancela o agendamento. Pode ser feito pelo aluno ou instrutor.
    /// </summary>
    public void Cancelar()
    {
        if (Status == StatusAgendamento.Concluido)
            throw new InvalidOperationException("Agendamentos concluídos não podem ser cancelados.");

        Status = StatusAgendamento.Cancelado;
    }

    /// <summary>
    /// Marca o agendamento como concluído após a realização da aula.
    /// </summary>
    public void Concluir()
    {
        if (Status != StatusAgendamento.Confirmado)
            throw new InvalidOperationException("Apenas agendamentos confirmados podem ser concluídos.");

        Status = StatusAgendamento.Concluido;
    }
}
