namespace ReparaLab.Application.FactoryMethod;

public interface INotificador
{
    string EnviarAviso(int ordenId, string cliente);
}