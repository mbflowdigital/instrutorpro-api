using Microsoft.AspNetCore.Mvc;

namespace InstrutorPro.API.Controllers;

/// <summary>
/// Controller de autenticação.
/// Gerencia login de instrutores e alunos com retorno de JWT.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    /// <summary>
    /// Autentica um instrutor ou aluno e retorna o token JWT.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        // TODO: Implementar LoginCommand com validação de senha e geração de JWT
        // 1. Buscar usuário pelo e-mail (Instrutor ou Aluno)
        // 2. Verificar senha (hash bcrypt)
        // 3. Gerar JWT com claims (id, role, nome)
        // 4. Retornar token + refresh token
        return Ok(new { message = "TODO: Implementar autenticação JWT" });
    }

    /// <summary>
    /// Renova o token JWT usando o refresh token.
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        // TODO: Implementar renovação de token
        return Ok(new { message = "TODO: Implementar refresh token" });
    }

    /// <summary>
    /// Revoga o refresh token (logout).
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        // TODO: Invalidar refresh token no Redis
        return NoContent();
    }
}

public record LoginRequest(string Email, string Senha);
public record RefreshTokenRequest(string RefreshToken);
