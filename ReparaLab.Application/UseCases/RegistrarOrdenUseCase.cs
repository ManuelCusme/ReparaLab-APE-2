namespace ReparaLab.Application.UseCases;

using ReparaLab.Application.DTOs;
using ReparaLab.Application.FactoryMethod;
using ReparaLab.Domain;
using ReparaLab.Domain.AbstractFactory;
using ReparaLab.Domain.Builder;
using ReparaLab.Domain.Notificaciones;
using ReparaLab.Domain.Repositories;

public class RegistrarOrdenUseCase
{
    private readonly IOrdenEscrituraRepository _escritura;
    private readonly PlanFactorySelector _planFactorySelector;
    private readonly INotificadorFactorySelector _notificadorFactorySelector;
    private readonly IBitacoraService _bitacora;

    public RegistrarOrdenUseCase(
        IOrdenEscrituraRepository escritura,
        PlanFactorySelector planFactorySelector,
        INotificadorFactorySelector notificadorFactorySelector,
        IBitacoraService bitacora)
    {
        _escritura = escritura;
        _planFactorySelector = planFactorySelector;
        _notificadorFactorySelector = notificadorFactorySelector;
        _bitacora = bitacora;
    }

    public OrdenDto Ejecutar(CrearOrdenDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Cliente))
            throw new ArgumentException("Cliente obligatorio.");
        if (string.IsNullOrWhiteSpace(dto.Falla))
            throw new ArgumentException("Describa la falla.");
        if (dto.Equipo is not ("LAPTOP" or "CELULAR" or "TABLET"))
            throw new ArgumentException("Equipo inválido. Use LAPTOP, CELULAR o TABLET.");
        if (dto.Servicio is not ("DIAGNOSTICO" or "MANTENIMIENTO"))
            throw new ArgumentException("Servicio inválido. Use DIAGNOSTICO o MANTENIMIENTO.");
        if (dto.Plan is not ("BASICO" or "PREMIUM"))
            throw new ArgumentException("Plan inválido. Use BASICO o PREMIUM.");
        if (dto.Notificacion is not ("EMAIL" or "SMS"))
            throw new ArgumentException("Aviso inválido. Use EMAIL o SMS.");

        // 1. Abstract Factory - seleccion de plan mediante selector inyectado
        IPlanFactory planFactory = _planFactorySelector.ObtenerFactory(dto.Plan);
        var tarifaPolicy = planFactory.CrearPoliticaTarifa();
        var garantiaPolicy = planFactory.CrearPoliticaGarantia();

        decimal total = tarifaPolicy.CalcularTotal(dto.Servicio, dto.Repuesto);
        int garantia = garantiaPolicy.ObtenerDiasGarantia();

        int siguienteId = _escritura.ObtenerSiguienteId();

        // 2. Builder - construccion de la orden
        OrdenReparacion orden = new OrdenReparacionBuilder()
            .ConId(siguienteId)
            .ConDatosCliente(dto.Cliente, dto.Equipo, dto.Falla)
            .ConServicioYPlan(dto.Servicio, dto.Plan)
            .ConNotificacionYRepuesto(dto.Notificacion, dto.Repuesto)
            .ConCalculos(total, garantia)
            .ConTareas(dto.Tareas)
            .Build();

        _escritura.Agregar(orden);

        // 3. Factory Method - envio de notificacion mediante selector inyectado
        NotificadorFactory notificadorFactory = _notificadorFactorySelector.ObtenerFactory(dto.Notificacion);
        INotificador notificador = notificadorFactory.CrearNotificador();
        _bitacora.RegistrarEvento(notificador.EnviarAviso(orden.Id, orden.Cliente));
        _bitacora.RegistrarEvento(
            $"Orden {orden.Id} guardada. Total ${orden.Total:0.00}. Garantía {orden.GarantiaDias} días.");

        if (orden.Tareas.Count > 0)
            _bitacora.RegistrarEvento($"Orden {orden.Id} usa una plantilla clonada con {orden.Tareas.Count} tarea(s).");

        return new OrdenDto(
            orden.Id, orden.Cliente, orden.Equipo, orden.Servicio,
            orden.Plan, orden.Total, orden.GarantiaDias, orden.Estado);
    }
}
