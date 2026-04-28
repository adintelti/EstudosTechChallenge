using FCG.Users.Application.DTOs;
using FCG.Users.Application.Exceptions;
using FCG.Users.Domain.Entities;
using FCG.Users.Infrastructure.Persistence;
using FCG.Users.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace FCG.Users.Application.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            AppDbContext context,
            IJwtService jwtService,
            ILogger<AuthService> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task RegisterAsync(RegisterRequest request)
        {
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
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user == null)
            {
                _logger.LogWarning("Tentativa de login inválida para o e-mail {Email}.", request.Email);
                throw new UnauthorizedAccessException("Usuário ou senha inválidos.");
            }

            var passwordValid = BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.PasswordHash);

            if (!passwordValid)
            {
                _logger.LogWarning("Tentativa de login inválida para o e-mail {Email}.", request.Email);
                throw new UnauthorizedAccessException("Usuário ou senha inválidos.");
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
