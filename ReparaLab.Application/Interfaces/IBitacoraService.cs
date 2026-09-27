namespace ReparaLab.Application.Interfaces;

public interface IBitacoraService
{
    void RegistrarEvento(string evento);
    IEnumerable<string> ObtenerEventos();
}