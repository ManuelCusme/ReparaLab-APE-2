namespace ReparaLab.Domain.Builder;

public class OrdenReparacionBuilder
{
    private readonly OrdenReparacion _orden = new();
    private bool _tieneId;
    private bool _tieneDatosCliente;
    private bool _tieneServicioYPlan;
    private bool _tieneNotificacionYRepuesto;
    private bool _tieneCalculos;

    public OrdenReparacionBuilder ConId(int id)
    {
        _orden.Id = id;
        _tieneId = true;
        return this;
    }

    public OrdenReparacionBuilder ConDatosCliente(string cliente, string equipo, string falla)
    {
        _orden.Cliente = cliente;
        _orden.Equipo = equipo;
        _orden.Falla = falla;
        _tieneDatosCliente = true;
        return this;
    }

    public OrdenReparacionBuilder ConServicioYPlan(string servicio, string plan)
    {
        _orden.Servicio = servicio;
        _orden.Plan = plan;
        _tieneServicioYPlan = true;
        return this;
    }

    public OrdenReparacionBuilder ConNotificacionYRepuesto(string notificacion, bool repuesto)
    {
        _orden.Notificacion = notificacion;
        _orden.Repuesto = repuesto;
        _tieneNotificacionYRepuesto = true;
        return this;
    }

    public OrdenReparacionBuilder ConCalculos(decimal total, int garantiaDias)
    {
        _orden.Total = total;
        _orden.GarantiaDias = garantiaDias;
        _tieneCalculos = true;
        return this;
    }

    public OrdenReparacionBuilder ConTareas(IEnumerable<string> tareas)
    {
        _orden.Tareas = new List<string>(tareas).AsReadOnly();
        return this;
    }

    public OrdenReparacion Build()
    {
        if (!_tieneId || !_tieneDatosCliente || !_tieneServicioYPlan || !_tieneNotificacionYRepuesto || !_tieneCalculos)
            throw new InvalidOperationException("La orden no está completa: faltan datos obligatorios antes de construirla.");

        return _orden;
    }
}
