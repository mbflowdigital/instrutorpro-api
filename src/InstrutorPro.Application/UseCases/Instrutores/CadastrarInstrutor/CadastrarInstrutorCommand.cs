using InstrutorPro.Domain.Enums;
using MediatR;

namespace InstrutorPro.Application.UseCases.Instrutores.CadastrarInstrutor;

/// <summary>
/// Command para cadastrar um novo instrutor na plataforma.
/// O instrutor ficará com status Pendente até que o credenciamento seja validado.
/// </summary>
public record CadastrarInstrutorCommand(
    string Nome,
    string CPF,
    string Email,
    string Telefone,
    string Estado,
    List<CategoriaHabilitacao> CategoriasHabilitacao,
    decimal ValorHoraAula,
    string? CidadeId = null,
    string? Bio = null
) : IRequest<Guid>;
