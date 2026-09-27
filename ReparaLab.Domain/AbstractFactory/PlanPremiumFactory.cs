namespace ReparaLab.Domain.AbstractFactory;

public class PlanPremiumFactory : IPlanFactory
{
    public IPoliticaTarifa CrearPoliticaTarifa() => new PoliticaTarifaPremium();
    public IPoliticaGarantia CrearPoliticaGarantia() => new PoliticaGarantiaPremium();
}