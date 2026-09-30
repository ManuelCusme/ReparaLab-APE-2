namespace ReparaLab.Infrastructure.Notificaciones;

using ReparaLab.Application.FactoryMethod;
using ReparaLab.Domain.Notificaciones;

public class SmsNotificadorFactory : NotificadorFactory
{
    public override INotificador CrearNotificador() => new SmsNotificador();
}
