using InstrutorPro.Application.Interfaces.Repositories;
using InstrutorPro.Domain.Entities;
using MediatR;

namespace InstrutorPro.Application.UseCases.Instrutores.CadastrarInstrutor;

/// <summary>
/// Handler para o cadastro de novos instrutores.
/// Valida duplicidade de CPF/e-mail e persiste o instrutor com status Pendente.
/// O instrutor só fica ativo após a validação do credenciamento (ver ValidarCredenciamentoHandler).
/// </summary>
public class CadastrarInstrutorHandler : IRequestHandler<CadastrarInstrutorCommand, Guid>
{
    private readonly IInstrutorRepository _instrutorRepository;

    public CadastrarInstrutorHandler(IInstrutorRepository instrutorRepository)
    {
        _instrutorRepository = instrutorRepository;
    }

    public async Task<Guid> Handle(CadastrarInstrutorCommand request, CancellationToken cancellationToken)
    {
        // Verifica se já existe instrutor com o mesmo CPF
        var instrutorExistenteCPF = await _instrutorRepository.GetByCPFAsync(request.CPF, cancellationToken);
        if (instrutorExistenteCPF is not null)
            throw new InvalidOperationException($"Já existe um instrutor cadastrado com o CPF informado.");

        // Verifica se já existe instrutor com o mesmo e-mail
        var instrutorExistenteEmail = await _instrutorRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (instrutorExistenteEmail is not null)
            throw new InvalidOperationException($"Já existe um instrutor cadastrado com o e-mail informado.");

        var instrutor = new Instrutor(
            nome: request.Nome,
            cpf: request.CPF,
            email: request.Email,
            telefone: request.Telefone,
            estado: request.Estado,
            categoriasHabilitacao: request.CategoriasHabilitacao,
            valorHoraAula: request.ValorHoraAula
        );

        await _instrutorRepository.AddAsync(instrutor, cancellationToken);

        return instrutor.Id;
    }
}
