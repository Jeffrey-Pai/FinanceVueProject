using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BankApi.DTOs;
using BankApi.Models;
using BankApi.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace BankApi.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IAccountRepository _accountRepo;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepo, IAccountRepository accountRepo, IConfiguration config)
    {
        _userRepo = userRepo;
        _accountRepo = accountRepo;
        _config = config;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepo.GetByUsernameAsync(request.Username) != null)
            throw new InvalidOperationException("Username already exists.");
        if (await _userRepo.GetByEmailAsync(request.Email) != null)
            throw new InvalidOperationException("Email already exists.");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };
        user = await _userRepo.CreateAsync(user);

        // Create a default checking account
        var account = new Account
        {
            AccountNumber = GenerateAccountNumber(),
            AccountType = "Checking",
            Balance = 0,
            UserId = user.Id
        };
        await _accountRepo.CreateAsync(account);

        return new AuthResponse(GenerateToken(user), user.Username, user.Id);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepo.GetByUsernameAsync(request.Username)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        return new AuthResponse(GenerateToken(user), user.Username, user.Id);
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpiryMinutes"]!)),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateAccountNumber() =>
        "ACC" + new Random().Next(100000000, 999999999).ToString();
}
