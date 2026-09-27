namespace ReparaLab.Application.UseCases;

using ReparaLab.Application.DTOs;
using ReparaLab.Application.Builder;
using ReparaLab.Application.FactoryMethod;
using ReparaLab.Application.Interfaces;
using ReparaLab.Domain.AbstractFactory;
using ReparaLab.Domain;

public class RegistrarOrdenUseCase
{
    private readonly IOrdenRepository _repository;
    private readonly IBitacoraService _bitacora;

    public RegistrarOrdenUseCase(IOrdenRepository repository, IBitacoraService bitacora)
    {
        _repository = repository;
        _bitacora = bitacora;
    }

    public OrdenDto Ejecutar(CrearOrdenDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Cliente))
            throw new ArgumentException("Cliente obligatorio.");
        if (string.IsNullOrWhiteSpace(dto.Falla))
            throw new ArgumentException("Describa la falla.");

        // 1. Abstract Factory — selección de plan
        IPlanFactory planFactory = dto.Plan == "PREMIUM"
            ? new PlanPremiumFactory()
            : new PlanBasicoFactory();

        var tarifaPolicy  = planFactory.CrearPoliticaTarifa();
        var garantiaPolicy = planFactory.CrearPoliticaGarantia();

        decimal total    = tarifaPolicy.CalcularTotal(dto.Servicio, dto.Repuesto);
        int     garantia = garantiaPolicy.ObtenerDiasGarantia();

        int siguienteId = _repository.ObtenerSiguienteId();

        // 2. Builder — construcción de la orden
        OrdenReparacion orden = new OrdenReparacionBuilder()
            .ConId(siguienteId)
            .ConDatosCliente(dto.Cliente, dto.Equipo, dto.Falla)
            .ConServicioYPlan(dto.Servicio, dto.Plan)
            .ConNotificacionYRepuesto(dto.Notificacion, dto.Repuesto)
            .ConCalculos(total, garantia)
            .Build();

        _repository.Agregar(orden);

        // 3. Factory Method — envío de notificación
        NotificadorFactory notificadorFactory = dto.Notificacion == "EMAIL"
            ? new EmailNotificadorFactory()
            : new SmsNotificadorFactory();

        INotificador notificador = notificadorFactory.CrearNotificador();
        _bitacora.RegistrarEvento(notificador.EnviarAviso(orden.Id, orden.Cliente));
        _bitacora.RegistrarEvento(
            $"Orden {orden.Id} guardada. Total ${orden.Total:0.00}. Garantía {orden.GarantiaDias} días.");

        return new OrdenDto(
            orden.Id, orden.Cliente, orden.Equipo, orden.Servicio,
            orden.Plan, orden.Total, orden.GarantiaDias, orden.Estado);
    }
}