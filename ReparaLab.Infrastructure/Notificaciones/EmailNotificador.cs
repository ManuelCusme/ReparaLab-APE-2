namespace ReparaLab.Infrastructure.Notificaciones;

using ReparaLab.Domain.Notificaciones;

public class EmailNotificador : INotificador
{
    public string EnviarAviso(int ordenId, string cliente) =>
        $"EMAIL simulado: orden {ordenId} registrada para {cliente}";
}
