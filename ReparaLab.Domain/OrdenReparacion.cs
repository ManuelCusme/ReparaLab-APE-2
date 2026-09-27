namespace ReparaLab.Domain;

public class OrdenReparacion
{
    public int Id { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public string Equipo { get; set; } = string.Empty;
    public string Falla { get; set; } = string.Empty;
    public string Servicio { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public string Notificacion { get; set; } = string.Empty;
    public bool Repuesto { get; set; }
    public decimal Total { get; set; }
    public int GarantiaDias { get; set; }
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