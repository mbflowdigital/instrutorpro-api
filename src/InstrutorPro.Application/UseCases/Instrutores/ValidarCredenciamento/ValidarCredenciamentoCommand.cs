using MediatR;

namespace InstrutorPro.Application.UseCases.Instrutores.ValidarCredenciamento;

/// <summary>
/// Command para validar o credenciamento de um instrutor junto ao Detran/Senatran.
/// Quando válido, o status do instrutor é atualizado para Ativo, tornando-o visível nas buscas.
/// </summary>
public record ValidarCredenciamentoCommand(
    Guid InstrutorId,
    string NumeroCredencial
) : IRequest<bool>;
