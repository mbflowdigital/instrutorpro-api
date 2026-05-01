using InstrutorPro.Application.Interfaces.Repositories;
using InstrutorPro.Application.Interfaces.Services;
using InstrutorPro.Domain.Enums;
using InstrutorPro.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace InstrutorPro.Infrastructure.Jobs;

/// <summary>
/// Job Hangfire executado semanalmente para revalidar os credenciamentos dos instrutores ativos.
/// Se o credenciamento de um instrutor for revogado pelo Detran, ele é automaticamente suspenso.
/// Configurado para executar toda segunda-feira às 03:00 (horário de Brasília).
/// </summary>
public class ValidacaoCredenciamentoJob
{
    private readonly IInstrutorRepository _instrutorRepository;
    private readonly IGovApiService _govApiService;
    private readonly ILogger<ValidacaoCredenciamentoJob> _logger;

    public ValidacaoCredenciamentoJob(
        IInstrutorRepository instrutorRepository,
        IGovApiService govApiService,
        ILogger<ValidacaoCredenciamentoJob> logger)
    {
        _instrutorRepository = instrutorRepository;
        _govApiService = govApiService;
        _logger = logger;
    }

    /// <summary>
    /// Executa a revalidação em lote de todos os instrutores ativos e pendentes.
    /// </summary>
    public async Task ExecutarAsync()
    {
        _logger.LogInformation("Iniciando job de validação de credenciamentos — {DataHora}", DateTime.UtcNow);

        var instrutores = await _instrutorRepository.GetAtivosAsync();
        var total = 0;
        var suspensos = 0;

        foreach (var instrutor in instrutores)
        {
            total++;

            try
            {
                if (instrutor.Credenciamento is null)
                {
                    _logger.LogWarning("Instrutor {Id} está ativo mas sem credenciamento registrado.", instrutor.Id);
                    continue;
                }

                var valido = await _govApiService.ValidarCredenciamentoAsync(
                    instrutor.CPF,
                    instrutor.Credenciamento.NumeroCredencial);

                if (!valido)
                {
                    instrutor.Suspender();
                    await _instrutorRepository.UpdateAsync(instrutor);
                    suspensos++;

                    _logger.LogWarning(
                        "Instrutor {Id} ({Nome}) suspenso por credenciamento inválido na API Gov.",
                        instrutor.Id, instrutor.Nome);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar credenciamento do instrutor {Id}", instrutor.Id);
            }
        }

        _logger.LogInformation(
            "Job de validação concluído — {Total} instrutores verificados, {Suspensos} suspensos.",
            total, suspensos);
    }
}
