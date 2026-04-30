using InstrutorPro.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace InstrutorPro.Infrastructure.Payment;

/// <summary>
/// Implementação da integração com o gateway de pagamentos Asaas.
/// Gerencia assinaturas recorrentes dos instrutores (Pix, Boleto, Cartão de Crédito).
/// Documentação Asaas: https://docs.asaas.com
/// </summary>
public class AsaasPaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AsaasPaymentService> _logger;
    private readonly string _apiKey;

    public AsaasPaymentService(HttpClient httpClient, IConfiguration configuration, ILogger<AsaasPaymentService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["Asaas:ApiKey"] ?? string.Empty;
    }

    /// <inheritdoc/>
    public async Task<string> CriarAssinaturaAsync(
        Guid instrutorId,
        string planoId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Criar o cliente no Asaas antes de criar a assinatura se não existir
            var payload = new
            {
                customer = instrutorId.ToString(), // ID do cliente no Asaas
                billingType = "PIX",               // PIX como padrão — pode ser BOLETO ou CREDIT_CARD
                value = ObterValorPlano(planoId),
                nextDueDate = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd"),
                cycle = "MONTHLY",
                description = $"Assinatura InstrutorPro — Plano {planoId}"
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "subscriptions");
            request.Headers.Add("access_token", _apiKey);
            request.Content = JsonContent.Create(payload);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AsaasSubscriptionResponse>(
                cancellationToken: cancellationToken);

            return result?.Id ?? throw new InvalidOperationException("Asaas não retornou ID da assinatura.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro ao criar assinatura no Asaas para instrutor {InstrutorId}", instrutorId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task CancelarAssinaturaAsync(string asaasSubscriptionId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"subscriptions/{asaasSubscriptionId}");
            request.Headers.Add("access_token", _apiKey);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro ao cancelar assinatura {SubscriptionId} no Asaas", asaasSubscriptionId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<string> ObterStatusAssinaturaAsync(string asaasSubscriptionId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"subscriptions/{asaasSubscriptionId}");
            request.Headers.Add("access_token", _apiKey);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AsaasSubscriptionResponse>(
                cancellationToken: cancellationToken);

            return result?.Status ?? "UNKNOWN";
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro ao obter status da assinatura {SubscriptionId} no Asaas", asaasSubscriptionId);
            throw;
        }
    }

    private static decimal ObterValorPlano(string planoId) =>
        // TODO: Buscar valor do plano do banco de dados ou configuração
        planoId switch
        {
            "basico" => 49.90m,
            "premium" => 99.90m,
            _ => 49.90m
        };

    private record AsaasSubscriptionResponse(string Id, string Status);
}
