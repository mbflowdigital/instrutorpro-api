using InstrutorPro.Application.Interfaces.Repositories;
using InstrutorPro.Domain.Entities;
using MediatR;

namespace InstrutorPro.Application.UseCases.Alunos.CadastrarAluno;

/// <summary>
/// Handler para o cadastro de novos alunos na plataforma.
/// Valida duplicidade de CPF e e-mail antes de persistir.
/// </summary>
public class CadastrarAlunoHandler : IRequestHandler<CadastrarAlunoCommand, Guid>
{
    private readonly IAlunoRepository _alunoRepository;

    public CadastrarAlunoHandler(IAlunoRepository alunoRepository)
    {
        _alunoRepository = alunoRepository;
    }

    public async Task<Guid> Handle(CadastrarAlunoCommand request, CancellationToken cancellationToken)
    {
        var alunoExistenteCPF = await _alunoRepository.GetByCPFAsync(request.CPF, cancellationToken);
        if (alunoExistenteCPF is not null)
            throw new InvalidOperationException("Já existe um aluno cadastrado com o CPF informado.");

        var alunoExistenteEmail = await _alunoRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (alunoExistenteEmail is not null)
            throw new InvalidOperationException("Já existe um aluno cadastrado com o e-mail informado.");

        var aluno = new Aluno(
            nome: request.Nome,
            cpf: request.CPF,
            email: request.Email,
            telefone: request.Telefone,
            estado: request.Estado,
            cidadeId: request.CidadeId
        );

        await _alunoRepository.AddAsync(aluno, cancellationToken);

        return aluno.Id;
    }
}
