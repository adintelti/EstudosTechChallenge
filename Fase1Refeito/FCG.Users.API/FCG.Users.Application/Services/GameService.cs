using FCG.Users.Application.DTOs;
using FCG.Users.Application.Exceptions;
using FCG.Users.Domain.Entities;
using FCG.Users.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FCG.Users.Application.Services
{
    public class GameService
    {
        private readonly AppDbContext _context;
        private readonly IValidator<CreateGameRequest> _createValidator;
        private readonly IValidator<UpdateGameRequest> _updateValidator;
        private readonly ILogger<GameService> _logger;

        public GameService(
            AppDbContext context,
            IValidator<CreateGameRequest> createvalidator,
            IValidator<UpdateGameRequest> updatevalidator,
            ILogger<GameService> logger)
        {
            _context = context;
            _createValidator = createvalidator;
            _updateValidator = updatevalidator;
            _logger = logger;
        }

        public async Task<GameResponse> CreateAsync(CreateGameRequest request)
        {
            var validationResult =
                await _createValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(x => x.ErrorMessage);

                throw new BusinessException(string.Join(", ", errors));
            }

            var game = new Game
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Price = request.Price
            };

            _context.Games.Add(game);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Game {Title} criado com sucesso.", game.Title);

            return new GameResponse
            {
                Id = game.Id,
                Title = game.Title,
                Price = game.Price
            };
        }

        public async Task<List<GameResponse>> GetAllAsync()
        {
            return await _context.Games
                .Select(x => new GameResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    Price = x.Price
                })
                .ToListAsync();
        }

        public async Task<GameResponse> GetByIdAsync(Guid id)
        {
            var game = await _context.Games
                .FirstOrDefaultAsync(x => x.Id == id);

            if (game == null)
            {
                _logger.LogWarning("Game não encontrado {id}.", id);
                throw new NotFoundException("Game não encontrado.");
            }

            return new GameResponse
            {
                Id = game.Id,
                Title = game.Title,
                Price = game.Price
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                _logger.LogWarning("Game não encontrado {id}.", id);
                throw new NotFoundException("Game não encontrado.");
            }

            _context.Games.Remove(game);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Game {GameId} removido.", game.Id);
        }

        public async Task<GameResponse> UpdateAsync(
            Guid id,
            UpdateGameRequest request)
        {
            var validationResult =
            await _updateValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(x => x.ErrorMessage);

                throw new ValidationException(string.Join(", ", errors));
            }

            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                _logger.LogWarning("Game não encontrado {id}.", id);
                throw new NotFoundException("Game não encontrado.");
            }

            game.Title = request.Title;
            game.Price = request.Price;

            await _context.SaveChangesAsync();

            return new GameResponse
            {
                Id = game.Id,
                Title = game.Title,
                Price = game.Price
            };
        }
    }
}
