namespace ReparaLab.Application.FactoryMethod;

public class EmailNotificador : INotificador
{
    public string EnviarAviso(int ordenId, string cliente) =>
        $"EMAIL simulado: orden {ordenId} registrada para {cliente}";
}