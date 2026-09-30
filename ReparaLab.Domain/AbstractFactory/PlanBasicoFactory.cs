namespace ReparaLab.Domain.AbstractFactory;

public class PlanBasicoFactory : IPlanFactory
{
    public IPoliticaTarifa CrearPoliticaTarifa() => new PoliticaTarifaBasico();
    public IPoliticaGarantia CrearPoliticaGarantia() => new PoliticaGarantiaBasico();
}