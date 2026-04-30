namespace InstrutorPro.Domain.Entities;

/// <summary>
/// Entidade que representa um aluno que busca instrutores de trânsito na plataforma.
/// </summary>
public class Aluno
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string CPF { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Telefone { get; private set; } = string.Empty;
    public string? CidadeId { get; private set; }
    public string Estado { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    // Navegação
    public ICollection<Agendamento> Agendamentos { get; private set; } = new List<Agendamento>();
    public ICollection<Avaliacao> Avaliacoes { get; private set; } = new List<Avaliacao>();

    private Aluno() { }

    public Aluno(
        string nome,
        string cpf,
        string email,
        string telefone,
        string estado,
        string? cidadeId = null)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        CPF = cpf;
        Email = email;
        Telefone = telefone;
        Estado = estado;
        CidadeId = cidadeId;
        CreatedAt = DateTime.UtcNow;
    }

    public void AtualizarPerfil(string nome, string telefone, string? cidadeId)
    {
        Nome = nome;
        Telefone = telefone;
        CidadeId = cidadeId;
    }
}
