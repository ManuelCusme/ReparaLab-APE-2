namespace ReparaLab.Infrastructure.Notificaciones;

using ReparaLab.Application.FactoryMethod;
using ReparaLab.Domain.Notificaciones;

public class EmailNotificadorFactory : NotificadorFactory
{
    public override INotificador CrearNotificador() => new EmailNotificador();
}
