namespace ReparaLab.Domain.AbstractFactory;

public class PlanFactorySelector
{
    public IPlanFactory ObtenerFactory(string plan) => plan switch
    {
        "BASICO" => new PlanBasicoFactory(),
        "PREMIUM" => new PlanPremiumFactory(),
        _ => throw new ArgumentException("Plan inválido. Use BASICO o PREMIUM.")
    };
}
