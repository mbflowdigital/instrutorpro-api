using InstrutorPro.Domain.Enums;
using InstrutorPro.Domain.ValueObjects;

namespace InstrutorPro.Domain.Entities;

/// <summary>
/// Entidade que representa um instrutor de trânsito credenciado pelo Detran.
/// Contém as regras de negócio relacionadas ao credenciamento e status do instrutor.
/// </summary>
public class Instrutor
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string CPF { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Telefone { get; private set; } = string.Empty;
    public string? CidadeId { get; private set; }
    public string Estado { get; private set; } = string.Empty;
    public string? Foto { get; private set; }
    public List<CategoriaHabilitacao> CategoriasHabilitacao { get; private set; } = new();
    public decimal ValorHoraAula { get; private set; }
    public string? Bio { get; private set; }
    public StatusInstrutor Status { get; private set; }
    public Credenciamento? Credenciamento { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navegação
    public ICollection<Agendamento> Agendamentos { get; private set; } = new List<Agendamento>();
    public ICollection<Avaliacao> Avaliacoes { get; private set; } = new List<Avaliacao>();
    public Assinatura? Assinatura { get; private set; }

    private Instrutor() { }

    public Instrutor(
        string nome,
        string cpf,
        string email,
        string telefone,
        string estado,
        List<CategoriaHabilitacao> categoriasHabilitacao,
        decimal valorHoraAula)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        CPF = cpf;
        Email = email;
        Telefone = telefone;
        Estado = estado;
        CategoriasHabilitacao = categoriasHabilitacao;
        ValorHoraAula = valorHoraAula;
        Status = StatusInstrutor.Pendente;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Ativa o instrutor após validação do credenciamento pela API governamental.
    /// </summary>
    public void Ativar(Credenciamento credenciamento)
    {
        Credenciamento = credenciamento.Validar();
        Status = StatusInstrutor.Ativo;
    }

    /// <summary>
    /// Suspende o instrutor quando o credenciamento é revogado ou há irregularidade.
    /// </summary>
    public void Suspender()
    {
        Status = StatusInstrutor.Suspenso;
        if (Credenciamento != null)
            Credenciamento = Credenciamento.Invalidar();
    }

    /// <summary>
    /// Inativa o instrutor voluntariamente.
    /// </summary>
    public void Inativar()
    {
        Status = StatusInstrutor.Inativo;
    }

    public void AtualizarPerfil(
        string nome,
        string telefone,
        string? cidadeId,
        string? foto,
        string? bio,
        decimal valorHoraAula)
    {
        Nome = nome;
        Telefone = telefone;
        CidadeId = cidadeId;
        Foto = foto;
        Bio = bio;
        ValorHoraAula = valorHoraAula;
    }
}
