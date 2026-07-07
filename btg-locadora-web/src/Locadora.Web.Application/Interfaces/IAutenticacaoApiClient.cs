using Locadora.Web.Domain.Models;

namespace Locadora.Web.Application.Interfaces;

public interface IAutenticacaoApiClient
{
    Task<Token> LoginAsync(LoginInput input, CancellationToken ct = default);
}
