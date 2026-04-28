using FCG.Users.Application.DTOs;
using FCG.Users.Application.Exceptions;
using FCG.Users.Domain.Entities;
using FCG.Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FCG.Users.Application.Services;

public class UserGameService
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserGameService> _logger;

    public UserGameService(AppDbContext context, ILogger<UserGameService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AcquireGameAsync(
        Guid userId,
        Guid gameId)
    {
        var userExists = await _context.Users
            .AnyAsync(x => x.Id == userId);

        if (!userExists)
        {
            _logger.LogWarning("User não encontrado {userId}.", userId);
            throw new NotFoundException("Usuário não encontrado.");
        }

        var game = await _context.Games
            .FirstOrDefaultAsync(x => x.Id == gameId);

        if (game == null)
        {
            _logger.LogWarning("Game não encontrado {gameId}.", gameId);
            throw new NotFoundException("Game não encontrado.");
        }

        var alreadyOwned = await _context.UserGames
            .AnyAsync(x =>
                x.UserId == userId &&
                x.GameId == gameId);

        if (alreadyOwned)
        {
            throw new BusinessException("Usuário já possui este game.");
        }

        var userGame = new UserGame
        {
            UserId = userId,
            GameId = gameId,
            AcquiredAt = DateTime.UtcNow
        };

        _context.UserGames.Add(userGame);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Usuário {UserId} adquiriu game {GameId}", userId, gameId);
    }

    public async Task<List<UserGameResponse>> GetLibraryAsync(Guid userId)
    {
        var library = await _context.UserGames
            .Where(x => x.UserId == userId)
            .Include(x => x.Game)
            .Select(x => new UserGameResponse
            {
                GameId = x.GameId,
                Title = x.Game.Title,
                Price = x.Game.Price,
                AcquiredAt = x.AcquiredAt
            })
            .ToListAsync();

        return library;
    }
}