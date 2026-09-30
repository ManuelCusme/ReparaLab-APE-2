namespace ReparaLab.Domain.Repositories;

public interface IOrdenLecturaRepository
{
    IEnumerable<OrdenReparacion> ObtenerTodas();
    OrdenReparacion? ObtenerPorId(int id);
}
