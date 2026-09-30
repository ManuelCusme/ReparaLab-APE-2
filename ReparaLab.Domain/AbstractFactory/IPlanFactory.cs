namespace ReparaLab.Domain.AbstractFactory;

public interface IPlanFactory
{
    IPoliticaTarifa CrearPoliticaTarifa();
    IPoliticaGarantia CrearPoliticaGarantia();
}