using FCG.Users.Application.Exceptions;
using FCG.Users.Application.Services;
using FCG.Users.Domain.Entities;
using FCG.Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.Users.Tests.Services;

public class UserGameServiceTests
{
    private readonly AppDbContext _context;
    private readonly Mock<ILogger<UserGameService>> _loggerMock;

    public UserGameServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        _loggerMock = new Mock<ILogger<UserGameService>>();
    }

    [Fact]
    public async Task AcquireGameAsync_Should_Add_Game_To_User_Library()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Adriano",
            Email = "adriano@gmail.com",
            PasswordHash = "hash",
            Role = "User"
        };

        var game = new Game
        {
            Id = Guid.NewGuid(),
            Title = "Resident Evil 4",
            Price = 249.90m
        };

        _context.Users.Add(user);

        _context.Games.Add(game);

        await _context.SaveChangesAsync();

        var service = new UserGameService(_context, _loggerMock.Object);

        await service.AcquireGameAsync(user.Id, game.Id);

        var userGame = await _context.UserGames
            .FirstOrDefaultAsync(x =>
                x.UserId == user.Id &&
                x.GameId == game.Id);

        Assert.NotNull(userGame);
    }

    [Fact]
    public async Task AcquireGameAsync_Should_Throw_Exception_When_User_Does_Not_Exist()
    {
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Title = "Resident Evil 4",
            Price = 249.90m
        };

        _context.Games.Add(game);

        await _context.SaveChangesAsync();

        var service = new UserGameService(_context, _loggerMock.Object);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.AcquireGameAsync(
                Guid.NewGuid(),
                game.Id));

        Assert.Equal(
            "Usuário não encontrado.",
            exception.Message);
    }

    [Fact]
    public async Task AcquireGameAsync_Should_Throw_Exception_When_Game_Does_Not_Exist()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Adriano",
            Email = "adriano@gmail.com",
            PasswordHash = "hash",
            Role = "User"
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        var service = new UserGameService(_context, _loggerMock.Object);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.AcquireGameAsync(
                user.Id,
                Guid.NewGuid()));

        Assert.Equal(
            "Game não encontrado.",
            exception.Message);
    }

    [Fact]
    public async Task AcquireGameAsync_Should_Throw_Exception_When_User_Already_Owns_Game()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Adriano",
            Email = "adriano@gmail.com",
            PasswordHash = "hash",
            Role = "User"
        };

        var game = new Game
        {
            Id = Guid.NewGuid(),
            Title = "Resident Evil 4",
            Price = 249.90m
        };

        _context.Users.Add(user);

        _context.Games.Add(game);

        _context.UserGames.Add(new UserGame
        {
            UserId = user.Id,
            GameId = game.Id,
            AcquiredAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        var service = new UserGameService(_context, _loggerMock.Object);

        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => service.AcquireGameAsync(
                user.Id,
                game.Id));

        Assert.Equal(
            "Usuário já possui este game.",
            exception.Message);
    }

    [Fact]
    public async Task GetLibraryAsync_Should_Return_User_Library()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Adriano",
            Email = "adriano@gmail.com",
            PasswordHash = "hash",
            Role = "User"
        };

        var game = new Game
        {
            Id = Guid.NewGuid(),
            Title = "Resident Evil 4",
            Price = 249.90m
        };

        _context.Users.Add(user);

        _context.Games.Add(game);

        _context.UserGames.Add(new UserGame
        {
            UserId = user.Id,
            GameId = game.Id,
            AcquiredAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        var service = new UserGameService(_context, _loggerMock.Object);

        var result = await service.GetLibraryAsync(user.Id);

        Assert.Single(result);

        Assert.Equal(game.Title, result[0].Title);
    }

    [Fact]
    public async Task GetLibraryAsync_Should_Return_Empty_List_When_User_Has_No_Games()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Adriano",
            Email = "adriano@gmail.com",
            PasswordHash = "hash",
            Role = "User"
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        var service = new UserGameService(_context, _loggerMock.Object);

        var result = await service.GetLibraryAsync(user.Id);

        Assert.Empty(result);
    }
}