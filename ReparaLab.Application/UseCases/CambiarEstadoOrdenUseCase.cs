namespace ReparaLab.Application.UseCases;

using ReparaLab.Application.Interfaces;

public class CambiarEstadoOrdenUseCase
{
    private readonly IOrdenRepository _repository;
    private readonly IBitacoraService _bitacora;

    public CambiarEstadoOrdenUseCase(IOrdenRepository repository, IBitacoraService bitacora)
    {
        _repository = repository;
        _bitacora   = bitacora;
    }

    public void Ejecutar(int id, string nuevoEstado)
    {
        var orden = _repository.ObtenerPorId(id)
            ?? throw new KeyNotFoundException("Orden no encontrada.");

        if (nuevoEstado == "FINALIZADA")
            orden.Finalizar();
        else if (nuevoEstado == "CANCELADA")
            orden.Cancelar();

        _bitacora.RegistrarEvento($"Orden {id} -> {nuevoEstado}");
    }
}