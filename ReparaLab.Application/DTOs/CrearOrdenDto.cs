namespace ReparaLab.Application.DTOs;

public record CrearOrdenDto(
    string Cliente,
    string Equipo,
    string Falla,
    string Servicio,
    string Plan,
    string Notificacion,
    bool Repuesto,
    List<string> Tareas
);
