using Microsoft.AspNetCore.Mvc;
using MonoPraksa.Model;
using MonoPraksa.Service;

namespace MonoPraksa.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        var success = await _authService.RegisterAsync(dto);
        if (!success)
            return BadRequest("Korisničko ime je zauzeto.");

        return Ok("Registracija uspješna.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var token = await _authService.LoginAsync(dto);
        if (token == null)
            return Unauthorized("Neispravni podaci za prijavu.");

        return Ok(new { Token = token });
    }
}
