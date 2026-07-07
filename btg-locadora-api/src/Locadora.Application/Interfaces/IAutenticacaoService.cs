using Locadora.Application.Dtos;

namespace Locadora.Application.Interfaces;

public interface IAutenticacaoService
{
    Task<TokenDto?> AutenticarAsync(LoginInputDto input);
}
