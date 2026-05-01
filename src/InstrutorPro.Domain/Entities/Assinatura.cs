using InstrutorPro.Domain.Enums;

namespace InstrutorPro.Domain.Entities;

/// <summary>
/// Entidade que representa a assinatura mensal do instrutor na plataforma.
/// Controla o acesso às funcionalidades premium e visibilidade nas buscas.
/// Integrada com o gateway de pagamento Asaas.
/// </summary>
public class Assinatura
{
    public Guid Id { get; private set; }
    public Guid InstrutorId { get; private set; }
    public string PlanoId { get; private set; } = string.Empty;
    public StatusAssinatura Status { get; private set; }
    public DateTime DataInicio { get; private set; }
    public DateTime DataProximoVencimento { get; private set; }

    /// <summary>
    /// ID da assinatura no gateway Asaas para reconciliação e gestão de cobranças.
    /// </summary>
    public string? AsaasSubscriptionId { get; private set; }

    // Navegação
    public Instrutor? Instrutor { get; private set; }

    private Assinatura() { }

    public Assinatura(
        Guid instrutorId,
        string planoId,
        DateTime dataInicio,
        DateTime dataProximoVencimento,
        string? asaasSubscriptionId = null)
    {
        Id = Guid.NewGuid();
        InstrutorId = instrutorId;
        PlanoId = planoId;
        Status = StatusAssinatura.Ativa;
        DataInicio = dataInicio;
        DataProximoVencimento = dataProximoVencimento;
        AsaasSubscriptionId = asaasSubscriptionId;
    }

    public void Cancelar()
    {
        Status = StatusAssinatura.Cancelada;
    }

    public void MarcarComoInadimplente()
    {
        Status = StatusAssinatura.Inadimplente;
    }

    public void Renovar(DateTime novoVencimento)
    {
        Status = StatusAssinatura.Ativa;
        DataProximoVencimento = novoVencimento;
    }
}
