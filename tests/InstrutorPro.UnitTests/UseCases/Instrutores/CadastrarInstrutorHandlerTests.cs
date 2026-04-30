using FluentAssertions;
using InstrutorPro.Application.Interfaces.Repositories;
using InstrutorPro.Application.UseCases.Instrutores.CadastrarInstrutor;
using InstrutorPro.Domain.Entities;
using InstrutorPro.Domain.Enums;
using Moq;

namespace InstrutorPro.UnitTests.UseCases.Instrutores;

/// <summary>
/// Testes unitários do CadastrarInstrutorHandler.
/// Valida os cenários de sucesso e falha no cadastro de instrutores.
/// </summary>
public class CadastrarInstrutorHandlerTests
{
    private readonly Mock<IInstrutorRepository> _instrutorRepositoryMock;
    private readonly CadastrarInstrutorHandler _handler;

    public CadastrarInstrutorHandlerTests()
    {
        _instrutorRepositoryMock = new Mock<IInstrutorRepository>();
        _handler = new CadastrarInstrutorHandler(_instrutorRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarGuid_QuandoInstrutorCadastradoComSucesso()
    {
        // Arrange
        var command = CriarCommandValido();

        _instrutorRepositoryMock
            .Setup(r => r.GetByCPFAsync(command.CPF, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Instrutor?)null);

        _instrutorRepositoryMock
            .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Instrutor?)null);

        _instrutorRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Instrutor>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        _instrutorRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Instrutor>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLancarExcecao_QuandoCPFJaCadastrado()
    {
        // Arrange
        var command = CriarCommandValido();
        var instrutorExistente = CriarInstrutorExistente();

        _instrutorRepositoryMock
            .Setup(r => r.GetByCPFAsync(command.CPF, It.IsAny<CancellationToken>()))
            .ReturnsAsync(instrutorExistente);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*CPF*");

        _instrutorRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Instrutor>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeveLancarExcecao_QuandoEmailJaCadastrado()
    {
        // Arrange
        var command = CriarCommandValido();
        var instrutorExistente = CriarInstrutorExistente();

        _instrutorRepositoryMock
            .Setup(r => r.GetByCPFAsync(command.CPF, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Instrutor?)null);

        _instrutorRepositoryMock
            .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(instrutorExistente);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*e-mail*");

        _instrutorRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Instrutor>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeveCriarInstrutorComStatusPendente()
    {
        // Arrange
        var command = CriarCommandValido();
        Instrutor? instrutorCapturado = null;

        _instrutorRepositoryMock
            .Setup(r => r.GetByCPFAsync(command.CPF, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Instrutor?)null);

        _instrutorRepositoryMock
            .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Instrutor?)null);

        _instrutorRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Instrutor>(), It.IsAny<CancellationToken>()))
            .Callback<Instrutor, CancellationToken>((i, _) => instrutorCapturado = i)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        instrutorCapturado.Should().NotBeNull();
        instrutorCapturado!.Status.Should().Be(StatusInstrutor.Pendente);
        instrutorCapturado.Nome.Should().Be(command.Nome);
        instrutorCapturado.Email.Should().Be(command.Email);
    }

    private static CadastrarInstrutorCommand CriarCommandValido() =>
        new(
            Nome: "João Silva",
            CPF: "12345678901",
            Email: "joao@teste.com",
            Telefone: "11999999999",
            Estado: "SP",
            CategoriasHabilitacao: new List<CategoriaHabilitacao> { CategoriaHabilitacao.B },
            ValorHoraAula: 80.00m
        );

    private static Instrutor CriarInstrutorExistente() =>
        new("Maria Santos", "98765432100", "maria@teste.com", "11888888888", "SP",
            new List<CategoriaHabilitacao> { CategoriaHabilitacao.B }, 90.00m);
}
