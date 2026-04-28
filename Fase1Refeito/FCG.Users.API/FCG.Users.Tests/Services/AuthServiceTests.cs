using FCG.Users.Application.DTOs;
using FCG.Users.Application.Services;
using FCG.Users.Infrastructure.Persistence;
using FCG.Users.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.Users.Tests.Services;

public class AuthServiceTests
{
    private readonly AppDbContext _context;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        _jwtServiceMock = new Mock<IJwtService>();
        _loggerMock = new Mock<ILogger<AuthService>>();
    }

    [Fact]
    public async Task RegisterAsync_Should_Create_User()
    {
        var service = new AuthService(
            _context,
            _jwtServiceMock.Object,
            _loggerMock.Object);

        var request = new RegisterRequest
        {
            Name = "Adriano",
            Email = "adriano@gmail.com",
            Password = "123456@Ab"
        };

        await service.RegisterAsync(request);

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        Assert.NotNull(user);
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Token()
    {
        var passwordHash =
            BCrypt.Net.BCrypt.HashPassword("123456@Ab");

        _context.Users.Add(new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Name = "Adriano",
            Email = "adriano@gmail.com",
            PasswordHash = passwordHash,
            Role = "User"
        });

        await _context.SaveChangesAsync();

        _jwtServiceMock
            .Setup(x => x.GenerateToken(It.IsAny<Domain.Entities.User>()))
            .Returns("fake-jwt-token");

        var service = new AuthService(
            _context,
            _jwtServiceMock.Object,
            _loggerMock.Object);

        var request = new LoginRequest
        {
            Email = "adriano@gmail.com",
            Password = "123456@Ab"
        };

        var result = await service.LoginAsync(request);

        Assert.NotNull(result);

        Assert.Equal("fake-jwt-token", result.Token);
    }

    [Fact]
    public async Task RegisterAsync_Should_Throw_Exception_When_User_Already_Exists()
    {
        _context.Users.Add(new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Name = "Adriano",
            Email = "adriano@gmail.com",
            PasswordHash = "hash",
            Role = "User"
        });

        await _context.SaveChangesAsync();

        var service = new AuthService(
            _context,
            _jwtServiceMock.Object,
            _loggerMock.Object);

        var request = new RegisterRequest
        {
            Name = "Novo User",
            Email = "adriano@gmail.com",
            Password = "123456@Ab"
        };

        var exception = await Assert.ThrowsAsync<Exception>(
            () => service.RegisterAsync(request));

        Assert.Equal(
            "Usuário já existe.",
            exception.Message);
    }

    [Fact]
    public async Task LoginAsync_Should_Throw_Exception_When_User_Does_Not_Exist()
    {
        var service = new AuthService(
            _context,
            _jwtServiceMock.Object,
            _loggerMock.Object);

        var request = new LoginRequest
        {
            Email = "naoexiste@gmail.com",
            Password = "123456@Ab"
        };

        var exception = await Assert.ThrowsAsync<Exception>(
            () => service.LoginAsync(request));

        Assert.Equal(
            "Usuário ou senha inválidos.",
            exception.Message);
    }

    [Fact]
    public async Task LoginAsync_Should_Throw_Exception_When_Password_Is_Invalid()
    {
        var passwordHash =
            BCrypt.Net.BCrypt.HashPassword("senha-correta");

        _context.Users.Add(new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Name = "Adriano",
            Email = "adriano@gmail.com",
            PasswordHash = passwordHash,
            Role = "User"
        });

        await _context.SaveChangesAsync();

        var service = new AuthService(
            _context,
            _jwtServiceMock.Object,
            _loggerMock.Object);

        var request = new LoginRequest
        {
            Email = "adriano@gmail.com",
            Password = "senha-errada"
        };

        var exception = await Assert.ThrowsAsync<Exception>(
            () => service.LoginAsync(request));

        Assert.Equal(
            "Usuário ou senha inválidos.",
            exception.Message);
    }
}