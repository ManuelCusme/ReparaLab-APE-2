namespace ReparaLab.Infrastructure.Notificaciones;

using ReparaLab.Application.FactoryMethod;

public class NotificadorFactorySelector : INotificadorFactorySelector
{
    public NotificadorFactory ObtenerFactory(string notificacion) => notificacion switch
    {
        "EMAIL" => new EmailNotificadorFactory(),
        "SMS" => new SmsNotificadorFactory(),
        _ => throw new ArgumentException("Aviso inválido. Use EMAIL o SMS.")
    };
}
