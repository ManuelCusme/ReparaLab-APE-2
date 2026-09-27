namespace ReparaLab.Domain.Prototype;

public class PlantillaReparacion : IPlantillaReparacion
{
    public string NombreServicio { get; set; } = string.Empty;
    public List<string> Tareas { get; set; } = new();

    public PlantillaReparacion(string nombreServicio, List<string> tareas)
    {
        NombreServicio = nombreServicio;
        Tareas = tareas;
    }

    // Copia profunda: instanciamos una nueva lista con los elementos existentes
    public IPlantillaReparacion Clonar()
    {
        var tareasClonadas = new List<string>(this.Tareas);
        return new PlantillaReparacion(this.NombreServicio, tareasClonadas);
    }
}