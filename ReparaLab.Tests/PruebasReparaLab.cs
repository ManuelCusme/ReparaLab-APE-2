namespace ReparaLab.Tests;

using System.Runtime.CompilerServices;
using ReparaLab.Application.DTOs;
using ReparaLab.Application.FactoryMethod;
using ReparaLab.Application.UseCases;
using ReparaLab.Domain;
using ReparaLab.Domain.AbstractFactory;
using ReparaLab.Domain.Prototype;
using ReparaLab.Infrastructure.Notificaciones;
using ReparaLab.Infrastructure.Repositories;
using ReparaLab.Infrastructure.Singleton;
using Xunit;

public class PruebasReparaLab
{
    private static RegistrarOrdenUseCase CrearRegistrarUseCase(OrdenMemoryRepository repo, IBitacoraService bitacora) =>
        new RegistrarOrdenUseCase(repo, new PlanFactorySelector(), new NotificadorFactorySelector(), bitacora);

    private static string ObtenerRutaProyecto(string carpetaProyecto, string archivoCsproj, [CallerFilePath] string archivoPrueba = "")
    {
        var raiz = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(archivoPrueba)!, ".."));
        return Path.Combine(raiz, carpetaProyecto, archivoCsproj);
    }

    [Fact]
    public void Prueba01_Ana_Basico_Email()
    {
        var repo = new OrdenMemoryRepository();
        var bitacora = BitacoraSingleton.Instancia;
        var useCase = CrearRegistrarUseCase(repo, bitacora);

        var dto = new CrearOrdenDto("Ana", "LAPTOP", "No enciende", "DIAGNOSTICO", "BASICO", "EMAIL", false, new List<string>());
        var resultado = useCase.Ejecutar(dto);

        Assert.Equal(20.00m, resultado.Total);
        Assert.Equal(30, resultado.GarantiaDias);
        Assert.Equal("PENDIENTE", resultado.Estado);
        Assert.Contains(bitacora.ObtenerEventos(), e => e.Contains("EMAIL simulado") && e.Contains($"orden {resultado.Id}"));
    }

    [Fact]
    public void Prueba02_Luis_Premium_Sms()
    {
        var repo = new OrdenMemoryRepository();
        var bitacora = BitacoraSingleton.Instancia;
        var useCase = CrearRegistrarUseCase(repo, bitacora);

        var dto = new CrearOrdenDto("Luis", "CELULAR", "No carga", "MANTENIMIENTO", "PREMIUM", "SMS", true, new List<string>());
        var resultado = useCase.Ejecutar(dto);

        Assert.Equal(58.75m, resultado.Total);
        Assert.Equal(90, resultado.GarantiaDias);
        Assert.Equal("PENDIENTE", resultado.Estado);
        Assert.Contains(bitacora.ObtenerEventos(), e => e.Contains("SMS simulado") && e.Contains($"orden {resultado.Id}"));
    }

    [Fact]
    public void Prueba03_ClienteVacio_Rechazado()
    {
        var repo = new OrdenMemoryRepository();
        var useCase = CrearRegistrarUseCase(repo, BitacoraSingleton.Instancia);
        var dto = new CrearOrdenDto("", "LAPTOP", "Falla", "DIAGNOSTICO", "BASICO", "EMAIL", false, new List<string>());

        var ex = Assert.Throws<ArgumentException>(() => useCase.Ejecutar(dto));
        Assert.Equal("Cliente obligatorio.", ex.Message);
    }

    [Fact]
    public void Prueba04_FallaVacia_Rechazado()
    {
        var repo = new OrdenMemoryRepository();
        var useCase = CrearRegistrarUseCase(repo, BitacoraSingleton.Instancia);
        var dto = new CrearOrdenDto("Ana", "LAPTOP", "", "DIAGNOSTICO", "BASICO", "EMAIL", false, new List<string>());

        var ex = Assert.Throws<ArgumentException>(() => useCase.Ejecutar(dto));
        Assert.Equal("Describa la falla.", ex.Message);
    }

    [Fact]
    public void Prueba05_RegistrarDosYListar()
    {
        var repo = new OrdenMemoryRepository();
        var bitacora = BitacoraSingleton.Instancia;
        var registrar = CrearRegistrarUseCase(repo, bitacora);
        var listar = new ListarOrdenesUseCase(repo);

        registrar.Ejecutar(new CrearOrdenDto("Ana", "LAPTOP", "Falla A", "DIAGNOSTICO", "BASICO", "EMAIL", false, new List<string>()));
        registrar.Ejecutar(new CrearOrdenDto("Luis", "CELULAR", "Falla B", "MANTENIMIENTO", "PREMIUM", "SMS", true, new List<string>()));

        var ordenes = listar.Ejecutar().ToList();
        Assert.Equal(2, ordenes.Count);
        Assert.Contains(ordenes, o => o.Cliente == "Ana");
        Assert.Contains(ordenes, o => o.Cliente == "Luis");
    }

    [Fact]
    public void Prueba06_FinalizarPendiente()
    {
        var repo = new OrdenMemoryRepository();
        var bitacora = BitacoraSingleton.Instancia;
        var registrar = CrearRegistrarUseCase(repo, bitacora);
        var cambiarEstado = new CambiarEstadoOrdenUseCase(repo, repo, bitacora);

        var resultado = registrar.Ejecutar(new CrearOrdenDto("Ana", "LAPTOP", "Falla", "DIAGNOSTICO", "BASICO", "EMAIL", false, new List<string>()));
        cambiarEstado.Ejecutar(resultado.Id, "FINALIZADA");

        var orden = repo.ObtenerPorId(resultado.Id);
        Assert.Equal("FINALIZADA", orden!.Estado);
        Assert.Contains(bitacora.ObtenerEventos(), e => e.Contains($"Orden {resultado.Id} -> FINALIZADA"));
    }

    [Fact]
    public void Prueba07_CancelarPendiente()
    {
        var repo = new OrdenMemoryRepository();
        var bitacora = BitacoraSingleton.Instancia;
        var registrar = CrearRegistrarUseCase(repo, bitacora);
        var cambiarEstado = new CambiarEstadoOrdenUseCase(repo, repo, bitacora);

        var resultado = registrar.Ejecutar(new CrearOrdenDto("Ana", "LAPTOP", "Falla", "DIAGNOSTICO", "BASICO", "EMAIL", false, new List<string>()));
        cambiarEstado.Ejecutar(resultado.Id, "CANCELADA");

        var orden = repo.ObtenerPorId(resultado.Id);
        Assert.Equal("CANCELADA", orden!.Estado);
        Assert.Contains(bitacora.ObtenerEventos(), e => e.Contains($"Orden {resultado.Id} -> CANCELADA"));
    }

    [Fact]
    public void Prueba08_FinalizarCancelada_Rechazado()
    {
        var repo = new OrdenMemoryRepository();
        var bitacora = BitacoraSingleton.Instancia;
        var registrar = CrearRegistrarUseCase(repo, bitacora);
        var cambiarEstado = new CambiarEstadoOrdenUseCase(repo, repo, bitacora);

        var resultado = registrar.Ejecutar(new CrearOrdenDto("Ana", "LAPTOP", "Falla", "DIAGNOSTICO", "BASICO", "EMAIL", false, new List<string>()));
        cambiarEstado.Ejecutar(resultado.Id, "CANCELADA");

        Assert.Throws<InvalidOperationException>(() => cambiarEstado.Ejecutar(resultado.Id, "FINALIZADA"));

        var orden = repo.ObtenerPorId(resultado.Id);
        Assert.Equal("CANCELADA", orden!.Estado);
    }

    [Fact]
    public void Prueba09_CancelarFinalizada_Rechazado()
    {
        var repo = new OrdenMemoryRepository();
        var bitacora = BitacoraSingleton.Instancia;
        var registrar = CrearRegistrarUseCase(repo, bitacora);
        var cambiarEstado = new CambiarEstadoOrdenUseCase(repo, repo, bitacora);

        var resultado = registrar.Ejecutar(new CrearOrdenDto("Ana", "LAPTOP", "Falla", "DIAGNOSTICO", "BASICO", "EMAIL", false, new List<string>()));
        cambiarEstado.Ejecutar(resultado.Id, "FINALIZADA");

        Assert.Throws<InvalidOperationException>(() => cambiarEstado.Ejecutar(resultado.Id, "CANCELADA"));

        var orden = repo.ObtenerPorId(resultado.Id);
        Assert.Equal("FINALIZADA", orden!.Estado);
    }

    [Fact]
    public void Prueba10_PlantillaDiagnostico_Editada_UsaCopiaEnLaOrden()
    {
        var catalogo = new CatalogoPlantillas();
        var obtenerPlantilla = new ObtenerPlantillaUseCase(catalogo);
        var repo = new OrdenMemoryRepository();
        var bitacora = BitacoraSingleton.Instancia;
        var registrar = CrearRegistrarUseCase(repo, bitacora);

        var plantilla = obtenerPlantilla.Ejecutar("DIAGNOSTICO");
        var tareasEditadas = new List<string>(plantilla.Tareas) { "Tarea agregada por el cliente" };

        var resultado = registrar.Ejecutar(new CrearOrdenDto("Ana", "LAPTOP", "Falla", "DIAGNOSTICO", "BASICO", "EMAIL", false, tareasEditadas));

        var orden = repo.ObtenerPorId(resultado.Id);
        Assert.Equal(tareasEditadas, orden!.Tareas);
        Assert.Contains("Tarea agregada por el cliente", orden.Tareas);
    }

    [Fact]
    public void Prueba11_ModificarCopia_NoAlteraPlantillaOriginal()
    {
        var catalogo = new CatalogoPlantillas();
        var obtenerPlantilla = new ObtenerPlantillaUseCase(catalogo);

        var copia1 = obtenerPlantilla.Ejecutar("DIAGNOSTICO");
        copia1.Tareas.Add("Tarea que no debe llegar a la plantilla original");

        var copia2 = obtenerPlantilla.Ejecutar("DIAGNOSTICO");

        Assert.DoesNotContain("Tarea que no debe llegar a la plantilla original", copia2.Tareas);
    }

    [Fact]
    public void Prueba12_BitacoraSingleton_MismaInstancia_YAcumulaEventos()
    {
        IBitacoraService consumidorA = BitacoraSingleton.Instancia;
        IBitacoraService consumidorB = BitacoraSingleton.Instancia;

        Assert.True(ReferenceEquals(consumidorA, consumidorB));

        int antes = consumidorA.ObtenerEventos().Count();
        consumidorB.RegistrarEvento("Evento de prueba 12");

        Assert.Equal(antes + 1, consumidorA.ObtenerEventos().Count());
        Assert.Contains(consumidorA.ObtenerEventos(), e => e.Contains("Evento de prueba 12"));
    }

    [Fact]
    public void Prueba13_NuevoRepositorio_EmpiezaVacio()
    {
        var repo = new OrdenMemoryRepository();
        Assert.Empty(repo.ObtenerTodas());
    }

    [Fact]
    public void Prueba14_Capas_SinReferenciasIndebidas_Y_FlujoUsaPatrones()
    {
        string domainCsproj = ObtenerRutaProyecto("ReparaLab.Domain", "ReparaLab.Domain.csproj");
        string applicationCsproj = ObtenerRutaProyecto("ReparaLab.Application", "ReparaLab.Application.csproj");

        string domainContenido = File.ReadAllText(domainCsproj);
        string applicationContenido = File.ReadAllText(applicationCsproj);

        Assert.DoesNotContain("<ProjectReference", domainContenido);
        Assert.Contains("ReparaLab.Domain.csproj", applicationContenido);
        Assert.DoesNotContain("ReparaLab.Infrastructure.csproj", applicationContenido);
        Assert.DoesNotContain("ReparaLab.WinForms.csproj", applicationContenido);

        var repo = new OrdenMemoryRepository();
        var bitacora = BitacoraSingleton.Instancia;
        var useCase = CrearRegistrarUseCase(repo, bitacora);

        var dto = new CrearOrdenDto("Flujo Test", "LAPTOP", "Prueba de flujo", "DIAGNOSTICO", "BASICO", "EMAIL", false, new List<string>());
        var resultado = useCase.Ejecutar(dto);

        var ordenGuardada = repo.ObtenerPorId(resultado.Id);
        Assert.NotNull(ordenGuardada);
        Assert.Equal(20.00m, ordenGuardada!.Total);
        Assert.Equal("PENDIENTE", ordenGuardada.Estado);
    }

    [Fact]
    public void Prueba15_OpcionesInvalidas_Rechazadas()
    {
        var repo = new OrdenMemoryRepository();
        var useCase = CrearRegistrarUseCase(repo, BitacoraSingleton.Instancia);

        Assert.Throws<ArgumentException>(() => useCase.Ejecutar(new CrearOrdenDto("Ana", "DRON", "Falla", "DIAGNOSTICO", "BASICO", "EMAIL", false, new List<string>())));
        Assert.Throws<ArgumentException>(() => useCase.Ejecutar(new CrearOrdenDto("Ana", "LAPTOP", "Falla", "PINTURA", "BASICO", "EMAIL", false, new List<string>())));
        Assert.Throws<ArgumentException>(() => useCase.Ejecutar(new CrearOrdenDto("Ana", "LAPTOP", "Falla", "DIAGNOSTICO", "GOLD", "EMAIL", false, new List<string>())));
        Assert.Throws<ArgumentException>(() => useCase.Ejecutar(new CrearOrdenDto("Ana", "LAPTOP", "Falla", "DIAGNOSTICO", "BASICO", "WHATSAPP", false, new List<string>())));
    }

    [Fact]
    public void Prueba16_EstadoDesconocido_RechazadoSinEvento()
    {
        var repo = new OrdenMemoryRepository();
        var bitacora = BitacoraSingleton.Instancia;
        var registrar = CrearRegistrarUseCase(repo, bitacora);
        var cambiarEstado = new CambiarEstadoOrdenUseCase(repo, repo, bitacora);

        var resultado = registrar.Ejecutar(new CrearOrdenDto("Ana", "LAPTOP", "Falla", "DIAGNOSTICO", "BASICO", "EMAIL", false, new List<string>()));

        int antes = bitacora.ObtenerEventos().Count();
        Assert.Throws<ArgumentException>(() => cambiarEstado.Ejecutar(resultado.Id, "EN_PROCESO"));

        var orden = repo.ObtenerPorId(resultado.Id);
        Assert.Equal("PENDIENTE", orden!.Estado);
        Assert.Equal(antes, bitacora.ObtenerEventos().Count());
    }
}
