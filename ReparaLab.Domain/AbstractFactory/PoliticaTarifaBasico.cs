namespace ReparaLab.Domain.AbstractFactory;

public class PoliticaTarifaBasico : IPoliticaTarifa
{
    public decimal CalcularTotal(string servicio, bool repuesto)
    {
        decimal basePrecio = TarifaBaseServicio.Obtener(servicio);
        return basePrecio + (repuesto ? 15m : 0m);
    }
}