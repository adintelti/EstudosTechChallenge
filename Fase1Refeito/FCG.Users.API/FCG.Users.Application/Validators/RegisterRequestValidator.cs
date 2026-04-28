using FCG.Users.Application.DTOs;
using FluentValidation;

namespace FCG.Users.Application.Validators.Games;

public class RegisterRequestValidator
    : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome é obrigatório.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email é obrigatório.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Senha é obrigatório.");
    }
}