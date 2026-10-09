using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MonoPraksa.Model;
using MonoPraksa.Repository;

namespace MonoPraksa.Service;

public class AuthService : IAuthService
{
    private readonly IUserRepository userRepository;
    private readonly IConfiguration configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        userRepository = userRepository;
        configuration = configuration;
    }

    public async Task<bool> RegisterAsync(UserRegisterDto dto)
    {
        var existing = await userRepository.GetByUsernameAsync(dto.Username);
        if (existing != null)
        {
            return false;
        }

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "User"
        };

        await userRepository.AddAsync(user);
        return true;
    }

    public async Task<string?> LoginAsync(UserLoginDto dto)
    {
        var user = await userRepository.GetByUsernameAsync(dto.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return null;
        }

        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
