using FCG.Users.Application.DTOs;
using FluentValidation;

namespace FCG.Users.Application.Validators.Games;

public class CreateGameRequestValidator
    : AbstractValidator<CreateGameRequest>
{
    public CreateGameRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Título é obrigatório.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Preço deve ser maior que zero.");
    }
}