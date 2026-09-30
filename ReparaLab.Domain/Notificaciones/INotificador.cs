namespace ReparaLab.Domain.Notificaciones;

public interface INotificador
{
    string EnviarAviso(int ordenId, string cliente);
}
