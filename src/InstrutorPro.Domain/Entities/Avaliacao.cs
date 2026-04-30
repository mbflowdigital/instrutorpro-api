namespace InstrutorPro.Domain.Entities;

/// <summary>
/// Entidade que representa a avaliação de um instrutor feita pelo aluno após a aula.
/// A nota deve ser entre 1 e 5.
/// </summary>
public class Avaliacao
{
    public Guid Id { get; private set; }
    public Guid AgendamentoId { get; private set; }
    public Guid InstrutorId { get; private set; }
    public Guid AlunoId { get; private set; }
    public int Nota { get; private set; }
    public string? Comentario { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navegação
    public Agendamento? Agendamento { get; private set; }
    public Instrutor? Instrutor { get; private set; }
    public Aluno? Aluno { get; private set; }

    private Avaliacao() { }

    public Avaliacao(
        Guid agendamentoId,
        Guid instrutorId,
        Guid alunoId,
        int nota,
        string? comentario = null)
    {
        if (nota < 1 || nota > 5)
            throw new ArgumentException("A nota deve ser entre 1 e 5.", nameof(nota));

        Id = Guid.NewGuid();
        AgendamentoId = agendamentoId;
        InstrutorId = instrutorId;
        AlunoId = alunoId;
        Nota = nota;
        Comentario = comentario;
        CreatedAt = DateTime.UtcNow;
    }
}
