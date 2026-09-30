namespace ReparaLab.Infrastructure.Repositories;

using ReparaLab.Domain;
using ReparaLab.Domain.Repositories;

public class OrdenMemoryRepository : IOrdenLecturaRepository, IOrdenEscrituraRepository
{
    private readonly List<OrdenReparacion> _ordenes = new();
    private int _siguienteId = 1;
    private readonly object _lock = new();

    public void Agregar(OrdenReparacion orden)
    {
        lock (_lock)
        {
            _ordenes.Add(orden);
        }
    }

    public void Actualizar(OrdenReparacion orden)
    {
        lock (_lock)
        {
            int index = _ordenes.FindIndex(x => x.Id == orden.Id);
            if (index >= 0)
                _ordenes[index] = orden;
        }
    }

    public IEnumerable<OrdenReparacion> ObtenerTodas()
    {
        lock (_lock)
        {
            return _ordenes.ToList();
        }
    }

    public OrdenReparacion? ObtenerPorId(int id)
    {
        lock (_lock)
        {
            return _ordenes.FirstOrDefault(x => x.Id == id);
        }
    }

    public int ObtenerSiguienteId()
    {
        lock (_lock)
        {
            return _siguienteId++;
        }
    }
}
