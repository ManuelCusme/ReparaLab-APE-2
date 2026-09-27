namespace ReparaLab.Application.FactoryMethod;

public class SmsNotificadorFactory : NotificadorFactory
{
    public override INotificador CrearNotificador() => new SmsNotificador();
}