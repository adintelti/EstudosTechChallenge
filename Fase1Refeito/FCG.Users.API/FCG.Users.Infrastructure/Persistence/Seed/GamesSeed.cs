using FCG.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.Users.Infrastructure.Persistence.Seed;

public static class GamesSeed
{
    public static async Task SeedAsync(AppDbContext context)
    {
        var gamesToSeed = new List<Game>
        {
            new() { Id = Guid.NewGuid(), Title = "Super Mario Bros", Price = 1 },
            new() { Id = Guid.NewGuid(), Title = "Pokémon Red", Price = 50 },
            new() { Id = Guid.NewGuid(), Title = "Tetris", Price = 20 },
            new() { Id = Guid.NewGuid(), Title = "The Legend of Zelda", Price = 60 },
            new() { Id = Guid.NewGuid(), Title = "Elden Ring", Price = 250 },
            new() { Id = Guid.NewGuid(), Title = "Hades", Price = 90 }
        };

        var existingTitles = await context.Games
                    .Where(g => gamesToSeed.Select(s => s.Title).Contains(g.Title))
                    .Select(g => g.Title)
                    .ToListAsync();

        var newGames = gamesToSeed
                    .Where(g => !existingTitles.Contains(g.Title))
                    .ToList();

        if (newGames.Any())
        {
            // 4. Inserção em lote (Bulk Insert do EF Core)
            await context.Games.AddRangeAsync(newGames);
            await context.SaveChangesAsync();
        }
    }
}