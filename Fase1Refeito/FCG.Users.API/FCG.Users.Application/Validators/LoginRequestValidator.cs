using FCG.Users.Application.DTOs;
using FluentValidation;

namespace FCG.Users.Application.Validators.Games;

public class LoginRequestValidator
    : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email é obrigatório.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Senha é obrigatório.");
    }
}