namespace ReparaLab.Domain;

public interface IBitacoraService
{
    void RegistrarEvento(string evento);
    IEnumerable<string> ObtenerEventos();
}
