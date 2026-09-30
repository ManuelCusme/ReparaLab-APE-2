namespace ReparaLab.Application.UseCases;

using ReparaLab.Domain;
using ReparaLab.Domain.Repositories;

public class CambiarEstadoOrdenUseCase
{
    private readonly IOrdenLecturaRepository _lectura;
    private readonly IOrdenEscrituraRepository _escritura;
    private readonly IBitacoraService _bitacora;

    public CambiarEstadoOrdenUseCase(
        IOrdenLecturaRepository lectura,
        IOrdenEscrituraRepository escritura,
        IBitacoraService bitacora)
    {
        _lectura = lectura;
        _escritura = escritura;
        _bitacora = bitacora;
    }

    public void Ejecutar(int id, string nuevoEstado)
    {
        if (nuevoEstado is not ("FINALIZADA" or "CANCELADA"))
            throw new ArgumentException("Estado inválido. Use FINALIZADA o CANCELADA.");

        var orden = _lectura.ObtenerPorId(id)
            ?? throw new KeyNotFoundException("Orden no encontrada.");

        if (nuevoEstado == "FINALIZADA")
            orden.Finalizar();
        else
            orden.Cancelar();

        _escritura.Actualizar(orden);
        _bitacora.RegistrarEvento($"Orden {id} -> {nuevoEstado}");
    }
}
