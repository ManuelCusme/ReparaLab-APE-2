namespace ReparaLab.Application.Builder;

using ReparaLab.Domain;

public class OrdenReparacionBuilder
{
    private readonly OrdenReparacion _orden = new();

    public OrdenReparacionBuilder ConId(int id)
    {
        _orden.Id = id;
        return this;
    }

    public OrdenReparacionBuilder ConDatosCliente(string cliente, string equipo, string falla)
    {
        _orden.Cliente = cliente;
        _orden.Equipo = equipo;
        _orden.Falla = falla;
        return this;
    }

    public OrdenReparacionBuilder ConServicioYPlan(string servicio, string plan)
    {
        _orden.Servicio = servicio;
        _orden.Plan = plan;
        return this;
    }

    public OrdenReparacionBuilder ConNotificacionYRepuesto(string notificacion, bool repuesto)
    {
        _orden.Notificacion = notificacion;
        _orden.Repuesto = repuesto;
        return this;
    }

    public OrdenReparacionBuilder ConCalculos(decimal total, int garantia)
    {
        _orden.Total = total;
        _orden.GarantiaDias = garantia;
        return this;
    }

    public OrdenReparacion Build() => _orden;
}