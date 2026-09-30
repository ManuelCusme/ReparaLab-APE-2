namespace ReparaLab.Application.FactoryMethod;

using ReparaLab.Domain.Notificaciones;

public abstract class NotificadorFactory
{
    public abstract INotificador CrearNotificador();
}