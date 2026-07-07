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

        RuleFor(p => p.Generos)
            .NotEmpty();

        RuleFor(p => p.Desenvolvedores)
            .NotEmpty();

        RuleFor(p => p.Publicadoras)
            .NotEmpty();
    }
}
