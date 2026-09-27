namespace ReparaLab.Application.Interfaces;

using ReparaLab.Domain;

public interface IOrdenRepository
{
    void Agregar(OrdenReparacion orden);
    IEnumerable<OrdenReparacion> ObtenerTodas();
    OrdenReparacion? ObtenerPorId(int id);
    int ObtenerSiguienteId();
}