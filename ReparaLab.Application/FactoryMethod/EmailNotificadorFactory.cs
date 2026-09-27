namespace ReparaLab.Application.FactoryMethod;

public class EmailNotificadorFactory : NotificadorFactory
{
    public override INotificador CrearNotificador() => new EmailNotificador();
}