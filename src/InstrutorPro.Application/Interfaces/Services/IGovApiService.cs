namespace InstrutorPro.Application.Interfaces.Services;

/// <summary>
/// Interface para integração com a API governamental do Senatran/Detran.
/// Responsável por validar se um instrutor está credenciado e autorizado a dar aulas.
/// </summary>
public interface IGovApiService
{
    /// <summary>
    /// Valida se o instrutor com o CPF e número de credencial informados
    /// está ativo e autorizado na base do Detran/Senatran.
    /// </summary>
    /// <param name="cpf">CPF do instrutor (apenas números).</param>
    /// <param name="numeroCredencial">Número do credenciamento emitido pelo Detran.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>True se o credenciamento é válido e ativo, false caso contrário.</returns>
    Task<bool> ValidarCredenciamentoAsync(string cpf, string numeroCredencial, CancellationToken cancellationToken = default);
}
