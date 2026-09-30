namespace ReparaLab.Infrastructure.Notificaciones;

using ReparaLab.Domain.Notificaciones;

public class SmsNotificador : INotificador
{
    public string EnviarAviso(int ordenId, string cliente) =>
        $"SMS simulado: orden {ordenId} registrada para {cliente}";
}
