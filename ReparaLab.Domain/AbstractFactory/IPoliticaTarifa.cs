namespace ReparaLab.Domain.AbstractFactory;

public interface IPoliticaTarifa
{
    decimal CalcularTotal(string servicio, bool repuesto);
}