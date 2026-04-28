using FCG.Users.Application.DTOs;
using FCG.Users.Application.Exceptions;
using FCG.Users.Application.Services;
using FCG.Users.Infrastructure.Persistence;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.Users.Tests.Services;

public class GameServiceTests
{
    private readonly AppDbContext _context;
    private readonly Mock<IValidator<CreateGameRequest>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateGameRequest>> _updateValidatorMock;
    private readonly Mock<ILogger<GameService>> _loggerMock;

    public GameServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        _createValidatorMock =
            new Mock<IValidator<CreateGameRequest>>();

        _updateValidatorMock =
            new Mock<IValidator<UpdateGameRequest>>();

        _loggerMock = new Mock<ILogger<GameService>>();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Game()
    {
        _createValidatorMock
        .Setup(x => x.ValidateAsync(
            It.IsAny<CreateGameRequest>(),
            default))
        .ReturnsAsync(new ValidationResult());

        var service = new GameService(
            _context,
            _createValidatorMock.Object,
            _updateValidatorMock.Object,
            _loggerMock.Object);

        var request = new CreateGameRequest
        {
            Title = "Resident Evil 4",
            Price = 249.90m
        };

        var result = await service.CreateAsync(request);

        Assert.NotNull(result);

        Assert.Equal(request.Title, result.Title);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Game()
    {
        var game = new Domain.Entities.Game
        {
            Id = Guid.NewGuid(),
            Title = "Old Title",
            Price = 100
        };

        _context.Games.Add(game);

        await _context.SaveChangesAsync();

        _updateValidatorMock
            .Setup(x => x.ValidateAsync(
                It.IsAny<UpdateGameRequest>(),
                default))
            .ReturnsAsync(new ValidationResult());

        var service = new GameService(
            _context,
            _createValidatorMock.Object,
            _updateValidatorMock.Object,
            _loggerMock.Object);

        var request = new UpdateGameRequest
        {
            Title = "New Title",
            Price = 200
        };

        var result = await service.UpdateAsync(
            game.Id,
            request);

        Assert.Equal("New Title", result.Title);

        Assert.Equal(200, result.Price);
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Game()
    {
        var game = new Domain.Entities.Game
        {
            Id = Guid.NewGuid(),
            Title = "Resident Evil",
            Price = 150
        };

        _context.Games.Add(game);

        await _context.SaveChangesAsync();

        var service = new GameService(
            _context,
            _createValidatorMock.Object,
            _updateValidatorMock.Object,
            _loggerMock.Object);

        await service.DeleteAsync(game.Id);

        var deletedGame = await _context.Games
            .FirstOrDefaultAsync(x => x.Id == game.Id);

        Assert.Null(deletedGame);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Throw_Exception_When_Game_Does_Not_Exist()
    {
        var service = new GameService(
            _context,
            _createValidatorMock.Object,
            _updateValidatorMock.Object,
            _loggerMock.Object);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetByIdAsync(Guid.NewGuid()));

        Assert.Equal(
            "Game não encontrado.",
            exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_Should_Throw_Exception_When_Game_Does_Not_Exist()
    {
        var service = new GameService(
            _context,
            _createValidatorMock.Object,
            _updateValidatorMock.Object,
            _loggerMock.Object);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.DeleteAsync(Guid.NewGuid()));

        Assert.Equal(
            "Game não encontrado.",
            exception.Message);
    }
}