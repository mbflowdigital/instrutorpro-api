using InstrutorPro.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace InstrutorPro.Infrastructure.GovApi;

/// <summary>
/// Implementação da integração com a API governamental do Senatran/Detran.
/// Valida se um instrutor está credenciado e autorizado a ministrar aulas práticas.
///
/// IMPORTANTE: A URL e autenticação exata da API devem ser confirmadas junto ao Senatran.
/// Esta implementação segue o padrão REST com autenticação via API Key.
/// </summary>
public class GovApiService : IGovApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GovApiService> _logger;
    private readonly string _apiKey;

    public GovApiService(HttpClient httpClient, IConfiguration configuration, ILogger<GovApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["GovApi:ApiKey"] ?? string.Empty;
    }

    /// <inheritdoc/>
    public async Task<bool> ValidarCredenciamentoAsync(
        string cpf,
        string numeroCredencial,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Ajustar o endpoint conforme documentação oficial do Senatran
            var url = $"instrutores/validar?cpf={cpf}&credencial={numeroCredencial}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("x-api-key", _apiKey);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "GovApi retornou status {StatusCode} ao validar credenciamento CPF {CPF}",
                    response.StatusCode, cpf);
                return false;
            }

            // TODO: Ajustar o modelo de resposta conforme documentação da API
            var result = await response.Content.ReadFromJsonAsync<GovApiValidacaoResponse>(
                cancellationToken: cancellationToken);

            return result?.Ativo ?? false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro ao chamar API Gov para validar credenciamento CPF {CPF}", cpf);
            // Em caso de falha na integração, não bloqueia o instrutor — retorna false para tentar novamente depois
            return false;
        }
    }

    /// <summary>
    /// Modelo de resposta da API governamental.
    /// </summary>
    private record GovApiValidacaoResponse(bool Ativo, string? Mensagem);
}
