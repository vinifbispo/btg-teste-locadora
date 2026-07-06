using Locadora.Application.Dtos;
using FluentValidation;

namespace Locadora.Api.Validators.Jogos;

public class JogoInputDtoValidator : AbstractValidator<JogoInputDto>
{
    public JogoInputDtoValidator()
    {
        RuleFor(p => p.Nome)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(p => p.ImagemCapa)
            .MaximumLength(500);

        RuleFor(p => p.Console)
            .IsInEnum();
    }
}
