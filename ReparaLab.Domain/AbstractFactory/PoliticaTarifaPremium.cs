namespace ReparaLab.Domain.AbstractFactory;

public class PoliticaTarifaPremium : IPoliticaTarifa
{
    public decimal CalcularTotal(string servicio, bool repuesto)
    {
        decimal basePrecio = servicio == "DIAGNOSTICO" ? 20m : 35m;
        decimal subtotalConRecargo = basePrecio * 1.25m; // 25% extra sobre el servicio base
        return subtotalConRecargo + (repuesto ? 15m : 0m);
    }
}