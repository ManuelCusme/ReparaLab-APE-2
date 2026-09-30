namespace ReparaLab.Application.UseCases;

using ReparaLab.Application.DTOs;
using ReparaLab.Domain.Repositories;

public class ListarOrdenesUseCase
{
    private readonly IOrdenLecturaRepository _lectura;

    public ListarOrdenesUseCase(IOrdenLecturaRepository lectura) => _lectura = lectura;

    public IEnumerable<OrdenDto> Ejecutar()
    {
        return _lectura.ObtenerTodas().Select(x => new OrdenDto(
            x.Id, x.Cliente, x.Equipo, x.Servicio,
            x.Plan, x.Total, x.GarantiaDias, x.Estado
        ));
    }
}
