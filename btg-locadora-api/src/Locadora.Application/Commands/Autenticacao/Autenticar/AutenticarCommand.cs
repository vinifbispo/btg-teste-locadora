using Locadora.Application.Dtos;
using MediatR;

namespace Locadora.Application.Commands.Autenticacao.Autenticar;

public record AutenticarCommand(LoginInputDto Input) : IRequest<TokenDto?>;
