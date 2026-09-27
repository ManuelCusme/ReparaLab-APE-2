namespace ReparaLab.Application.FactoryMethod;

public class SmsNotificador : INotificador
{
    public string EnviarAviso(int ordenId, string cliente) =>
        $"SMS simulado: orden {ordenId} registrada para {cliente}";
}