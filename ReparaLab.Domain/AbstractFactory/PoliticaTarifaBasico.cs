namespace ReparaLab.Domain.AbstractFactory;

public class PoliticaTarifaBasico : IPoliticaTarifa
{
    public decimal CalcularTotal(string servicio, bool repuesto)
    {
        decimal basePrecio = servicio == "DIAGNOSTICO" ? 20m : 35m;
        return basePrecio + (repuesto ? 15m : 0m);
    }
}