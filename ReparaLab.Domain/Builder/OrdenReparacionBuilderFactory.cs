namespace ReparaLab.Domain.Builder;

public sealed class OrdenReparacionBuilderFactory : IOrdenReparacionBuilderFactory
{
    public OrdenReparacionBuilder Crear() => new();
}
