namespace ReparaLab.Infrastructure.Repositories;

using ReparaLab.Application.Interfaces;
using ReparaLab.Domain;
using ReparaLab.Domain.Repositories;

public class OrdenMemoryRepository : IOrdenRepository
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