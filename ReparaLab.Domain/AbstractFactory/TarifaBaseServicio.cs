namespace ReparaLab.Domain.AbstractFactory;

internal static class TarifaBaseServicio
{
    private const decimal PrecioDiagnostico = 20m;
    private const decimal PrecioMantenimiento = 35m;

    public static decimal Obtener(string servicio) =>
        servicio == "DIAGNOSTICO" ? PrecioDiagnostico : PrecioMantenimiento;
}
