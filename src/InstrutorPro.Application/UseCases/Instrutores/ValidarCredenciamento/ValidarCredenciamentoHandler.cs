using InstrutorPro.Application.Interfaces.Repositories;
using InstrutorPro.Application.Interfaces.Services;
using InstrutorPro.Domain.ValueObjects;
using MediatR;

namespace InstrutorPro.Application.UseCases.Instrutores.ValidarCredenciamento;

/// <summary>
/// Handler para validação do credenciamento do instrutor via API governamental.
/// Consulta a API do Senatran/Detran e ativa ou suspende o instrutor conforme resultado.
/// Este handler também é chamado pelo job semanal de revalidação (ValidacaoCredenciamentoJob).
/// </summary>
public class ValidarCredenciamentoHandler : IRequestHandler<ValidarCredenciamentoCommand, bool>
{
    private readonly IInstrutorRepository _instrutorRepository;
    private readonly IGovApiService _govApiService;

    public ValidarCredenciamentoHandler(
        IInstrutorRepository instrutorRepository,
        IGovApiService govApiService)
    {
        _instrutorRepository = instrutorRepository;
        _govApiService = govApiService;
    }

    public async Task<bool> Handle(ValidarCredenciamentoCommand request, CancellationToken cancellationToken)
    {
        var instrutor = await _instrutorRepository.GetByIdAsync(request.InstrutorId, cancellationToken)
            ?? throw new KeyNotFoundException($"Instrutor com ID {request.InstrutorId} não encontrado.");

        // Consulta a API do governo para verificar se o credenciamento é válido
        var credenciamentoValido = await _govApiService.ValidarCredenciamentoAsync(
            instrutor.CPF,
            request.NumeroCredencial,
            cancellationToken);

        if (credenciamentoValido)
        {
            // Ativa o instrutor com o credenciamento validado
            var credenciamento = new Credenciamento(request.NumeroCredencial, DateTime.UtcNow);
            instrutor.Ativar(credenciamento);
        }
        else
        {
            // Se o credenciamento não é mais válido, suspende o instrutor
            instrutor.Suspender();
        }

        await _instrutorRepository.UpdateAsync(instrutor, cancellationToken);

        return credenciamentoValido;
    }
}
