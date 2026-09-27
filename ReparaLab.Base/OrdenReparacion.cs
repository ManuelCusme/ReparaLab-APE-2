namespace ReparaLab.Base;

// Modelo intencionalmente anemico para analizar y mejorar en el APE 02.
public class OrdenReparacion
{
    public int Id { get; set; }
    public string Cliente { get; set; } = "";
    public string Equipo { get; set; } = "";
    public string Falla { get; set; } = "";
    public string Servicio { get; set; } = "";
    public string Plan { get; set; } = "";
    public string Notificacion { get; set; } = "";
    public bool Repuesto { get; set; }
    public decimal Total { get; set; }
    public int GarantiaDias { get; set; }
    public string Estado { get; set; } = "PENDIENTE";
}
