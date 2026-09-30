namespace ReparaLab.Domain.Prototype;

public class CatalogoPlantillas
{
    private readonly Dictionary<string, PlantillaReparacion> _plantillas;

    public CatalogoPlantillas()
    {
        _plantillas = new Dictionary<string, PlantillaReparacion>
        {
            ["DIAGNOSTICO"] = new PlantillaReparacion("DIAGNOSTICO", new List<string>
            {
                "Revisión de hardware",
                "Diagnóstico de software",
                "Informe de resultados"
            }),
            ["MANTENIMIENTO"] = new PlantillaReparacion("MANTENIMIENTO", new List<string>
            {
                "Limpieza interna",
                "Actualización de software",
                "Prueba de rendimiento"
            })
        };
    }

    public PlantillaReparacion Obtener(string servicio)
    {
        if (!_plantillas.TryGetValue(servicio, out var plantilla))
            throw new ArgumentException("Plantilla inválida. Use DIAGNOSTICO o MANTENIMIENTO.");
        return plantilla;
    }
}
