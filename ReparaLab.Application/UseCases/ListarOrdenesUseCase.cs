namespace ReparaLab.Application.UseCases;

using ReparaLab.Application.DTOs;
using ReparaLab.Application.Interfaces;

public class ListarOrdenesUseCase
{
    private readonly IOrdenRepository _repository;

    public ListarOrdenesUseCase(IOrdenRepository repository) => _repository = repository;

    public IEnumerable<OrdenDto> Ejecutar()
    {
        return _repository.ObtenerTodas().Select(x => new OrdenDto(
            x.Id, x.Cliente, x.Equipo, x.Servicio,
            x.Plan, x.Total, x.GarantiaDias, x.Estado
        ));
    }
}