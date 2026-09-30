namespace ReparaLab.Domain;

public class OrdenReparacion
{
    internal OrdenReparacion() { }

    public int Id { get; internal set; }
    public string Cliente { get; internal set; } = string.Empty;
    public string Equipo { get; internal set; } = string.Empty;
    public string Falla { get; internal set; } = string.Empty;
    public string Servicio { get; internal set; } = string.Empty;
    public string Plan { get; internal set; } = string.Empty;
    public string Notificacion { get; internal set; } = string.Empty;
    public bool Repuesto { get; internal set; }
    public decimal Total { get; internal set; }
    public int GarantiaDias { get; internal set; }
    public IReadOnlyList<string> Tareas { get; internal set; } = Array.Empty<string>();
    public string Estado { get; private set; } = "PENDIENTE";

    public void Finalizar()
    {
        if (Estado != "PENDIENTE")
            throw new InvalidOperationException("Solo se puede finalizar una orden en estado PENDIENTE.");
        Estado = "FINALIZADA";
    }

    public void Cancelar()
    {
        if (Estado != "PENDIENTE")
            throw new InvalidOperationException("Solo se puede cancelar una orden en estado PENDIENTE.");
        Estado = "CANCELADA";
    }
}
