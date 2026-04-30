namespace InstrutorPro.Domain.ValueObjects;

/// <summary>
/// Value Object que representa o credenciamento de um instrutor junto ao Detran/Senatran.
/// Encapsula as regras de validação do número de credencial.
/// </summary>
public sealed class Credenciamento
{
    public string NumeroCredencial { get; private set; } = null!;
    public DateTime DataCredenciamento { get; private set; }
    public bool Validado { get; private set; }

    private Credenciamento() { }

    public Credenciamento(string numeroCredencial, DateTime dataCredenciamento, bool validado = false)
    {
        if (string.IsNullOrWhiteSpace(numeroCredencial))
            throw new ArgumentException("Número de credencial não pode ser vazio.", nameof(numeroCredencial));

        NumeroCredencial = numeroCredencial.Trim().ToUpperInvariant();
        DataCredenciamento = dataCredenciamento;
        Validado = validado;
    }

    /// <summary>
    /// Marca o credenciamento como validado pela API governamental.
    /// </summary>
    public Credenciamento Validar() =>
        new(NumeroCredencial, DataCredenciamento, validado: true);

    /// <summary>
    /// Marca o credenciamento como inválido (ex: revogado pelo Detran).
    /// </summary>
    public Credenciamento Invalidar() =>
        new(NumeroCredencial, DataCredenciamento, validado: false);

    public override string ToString() => NumeroCredencial;

    public override bool Equals(object? obj) =>
        obj is Credenciamento other && NumeroCredencial == other.NumeroCredencial;

    public override int GetHashCode() => NumeroCredencial.GetHashCode();
}
