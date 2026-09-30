namespace ReparaLab.Infrastructure.Singleton;

using ReparaLab.Domain;

public sealed class BitacoraSingleton : IBitacoraService
{
    private static readonly BitacoraSingleton _instancia = new();
    private readonly List<string> _eventos = new();
    private readonly object _lock = new();

    // Constructor privado para impedir instanciación externa
    private BitacoraSingleton() { }

    public static BitacoraSingleton Instancia => _instancia;

    public void RegistrarEvento(string evento)
    {
        lock (_lock)
        {
            string registro = $"{DateTime.Now:HH:mm:ss} | {evento}";
            _eventos.Add(registro);
        }
    }

    public IEnumerable<string> ObtenerEventos()
    {
        lock (_lock)
        {
            return _eventos.ToList();
        }
    }
}