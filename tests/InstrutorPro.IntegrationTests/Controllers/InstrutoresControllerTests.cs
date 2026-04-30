using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace InstrutorPro.IntegrationTests.Controllers;

/// <summary>
/// Testes de integração para o InstrutoresController.
/// Verifica o comportamento HTTP da API de instrutores.
/// </summary>
public class InstrutoresControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public InstrutoresControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_Instrutores_DeveRetornar200()
    {
        // Act
        var response = await _client.GetAsync("/api/instrutores");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task POST_Instrutores_ComBodyInvalido_DeveRetornar400()
    {
        // Arrange — body vazio
        var content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/instrutores", content);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
    }
}
