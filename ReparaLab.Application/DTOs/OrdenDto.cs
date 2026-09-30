namespace ReparaLab.Application.DTOs;

public record OrdenDto(
    int Id,
    string Cliente,
    string Equipo,
    string Servicio,
    string Plan,
    decimal Total,
    int GarantiaDias,
    string Estado
);