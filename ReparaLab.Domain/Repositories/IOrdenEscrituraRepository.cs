namespace ReparaLab.Domain.Repositories;

public interface IOrdenEscrituraRepository
{
    void Agregar(OrdenReparacion orden);
    void Actualizar(OrdenReparacion orden);
    int ObtenerSiguienteId();
}
