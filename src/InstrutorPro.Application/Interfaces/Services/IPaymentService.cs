namespace InstrutorPro.Application.Interfaces.Services;

/// <summary>
/// Interface para integração com o gateway de pagamentos Asaas.
/// Gerencia assinaturas recorrentes e cobranças dos instrutores (Pix, Boleto).
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Cria uma assinatura recorrente no Asaas para o instrutor.
    /// </summary>
    Task<string> CriarAssinaturaAsync(Guid instrutorId, string planoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancela uma assinatura no Asaas.
    /// </summary>
    Task CancelarAssinaturaAsync(string asaasSubscriptionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica o status atual de uma assinatura no Asaas.
    /// </summary>
    Task<string> ObterStatusAssinaturaAsync(string asaasSubscriptionId, CancellationToken cancellationToken = default);
}
