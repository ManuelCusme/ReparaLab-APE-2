namespace ReparaLab.Application.UseCases;

using ReparaLab.Application.DTOs;
using ReparaLab.Domain.Prototype;

public class ObtenerPlantillaUseCase
{
    private readonly CatalogoPlantillas _catalogo;

    public ObtenerPlantillaUseCase(CatalogoPlantillas catalogo) => _catalogo = catalogo;

    public PlantillaDto Ejecutar(string servicio)
    {
        if (servicio is not ("DIAGNOSTICO" or "MANTENIMIENTO"))
            throw new ArgumentException("Plantilla inválida. Use DIAGNOSTICO o MANTENIMIENTO.");

        var original = _catalogo.Obtener(servicio);
        var copia = (PlantillaReparacion)original.Clonar();
        return new PlantillaDto(copia.NombreServicio, copia.Tareas);
    }
}
