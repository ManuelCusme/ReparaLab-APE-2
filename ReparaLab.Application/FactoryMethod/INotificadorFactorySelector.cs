namespace ReparaLab.Application.FactoryMethod;

public interface INotificadorFactorySelector
{
    NotificadorFactory ObtenerFactory(string notificacion);
}
