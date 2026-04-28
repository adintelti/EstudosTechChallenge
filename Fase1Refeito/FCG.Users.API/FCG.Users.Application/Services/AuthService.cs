using FCG.Users.Application.DTOs;
using FCG.Users.Application.Exceptions;
using FCG.Users.Domain.Entities;
using FCG.Users.Infrastructure.Persistence;
using FCG.Users.Infrastructure.Security;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FCG.Users.Application.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<LoginRequest> _loginValidator;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            AppDbContext context,
            IJwtService jwtService,
            IValidator<RegisterRequest> registerValidator,
            IValidator<LoginRequest> loginValidator,
            ILogger<AuthService> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _logger = logger;
        }

        public async Task RegisterAsync(RegisterRequest request)
        {
            var validationResult = await _registerValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(x => x.ErrorMessage);

                throw new BusinessException(string.Join(", ", errors));
            }

            var userExists = await _context.Users
                .AnyAsync(x => x.Email == request.Email);

            if (userExists)
            {
                _logger.LogWarning("Usuário já existe com o e-mail {Email}.", request.Email);
                throw new BusinessException("Usuário já existe.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "User"
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuário {Email} criado com sucesso.", user.Email);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var validationResult = await _loginValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(x => x.ErrorMessage);

                throw new BusinessException(string.Join(", ", errors));
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user == null)
            {
                _logger.LogWarning("Tentativa de login inválida para o e-mail {Email}.", request.Email);
                throw new UnauthorizedException("Usuário ou senha inválidos.");
            }

            var passwordValid = BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.PasswordHash);

            if (!passwordValid)
            {
                _logger.LogWarning("Tentativa de login inválida para o e-mail {Email}.", request.Email);
                throw new UnauthorizedException("Usuário ou senha inválidos.");
            }

            _logger.LogInformation("Usuário {Email} realizou login com sucesso.", user.Email);

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Token = token
            };
        }
    }
}
