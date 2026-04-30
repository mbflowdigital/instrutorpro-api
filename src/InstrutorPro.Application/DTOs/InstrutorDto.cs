using InstrutorPro.Domain.Enums;

namespace InstrutorPro.Application.DTOs;

/// <summary>
/// DTO de retorno com os dados públicos de um instrutor.
/// </summary>
public record InstrutorDto(
    Guid Id,
    string Nome,
    string Email,
    string Telefone,
    string? CidadeId,
    string Estado,
    string? Foto,
    List<CategoriaHabilitacao> CategoriasHabilitacao,
    decimal ValorHoraAula,
    string? Bio,
    StatusInstrutor Status,
    DateTime CreatedAt
);
