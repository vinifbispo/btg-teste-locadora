using Locadora.Application.Dtos;

namespace Locadora.Application.Interfaces;

public interface IDesenvolvedorService
{
    Task<IEnumerable<DesenvolvedorDto>> BuscarPorNomeAsync(string termo);
}
