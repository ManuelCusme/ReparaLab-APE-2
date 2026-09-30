namespace ReparaLab.Tests;

using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using ReparaLab.Application.DTOs;
using ReparaLab.Application.FactoryMethod;
using ReparaLab.Application.UseCases;
using ReparaLab.Domain;
using ReparaLab.Domain.AbstractFactory;
using ReparaLab.Domain.Builder;
using ReparaLab.Domain.Prototype;
using ReparaLab.Domain.Repositories;
using ReparaLab.Infrastructure.Notificaciones;
using ReparaLab.Infrastructure.Repositories;
using ReparaLab.Infrastructure.Singleton;
using Xunit;

public class PruebasReparaLab
{
    private static RegistrarOrdenUseCase CrearRegistrarUseCase(OrdenMemoryRepository repo, IBitacoraService bitacora) =>
        new RegistrarOrdenUseCase(repo, new PlanFactorySelector(), new OrdenReparacionBuilderFactory(), new NotificadorFactorySelector(), bitacora);

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
        int eventosPrevios = bitacora.ObtenerEventos().Count();
        var useCase = CrearRegistrarUseCase(repo, bitacora);

        var dto = new CrearOrdenDto("Ana", "LAPTOP", "No enciende", "DIAGNOSTICO", "BASICO", "EMAIL", false, new List<string>());
        var resultado = useCase.Ejecutar(dto);

        Assert.Equal(20.00m, resultado.Total);
        Assert.Equal(30, resultado.GarantiaDias);
        Assert.Equal("PENDIENTE", resultado.Estado);
        var eventosNuevos = bitacora.ObtenerEventos().Skip(eventosPrevios);
        Assert.Contains(eventosNuevos, e => e.Contains("EMAIL simulado") && e.Contains($"orden {resultado.Id}"));
    }

    [Fact]
    public void Prueba02_Luis_Premium_Sms()
    {
        var repo = new OrdenMemoryRepository();
        var bitacora = BitacoraSingleton.Instancia;
        int eventosPrevios = bitacora.ObtenerEventos().Count();
        var useCase = CrearRegistrarUseCase(repo, bitacora);

        var dto = new CrearOrdenDto("Luis", "CELULAR", "No carga", "MANTENIMIENTO", "PREMIUM", "SMS", true, new List<string>());
        var resultado = useCase.Ejecutar(dto);

        Assert.Equal(58.75m, resultado.Total);
        Assert.Equal(90, resultado.GarantiaDias);
        Assert.Equal("PENDIENTE", resultado.Estado);
        var eventosNuevos = bitacora.ObtenerEventos().Skip(eventosPrevios);
        Assert.Contains(eventosNuevos, e => e.Contains("SMS simulado") && e.Contains($"orden {resultado.Id}"));
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
        int eventosPrevios = bitacora.ObtenerEventos().Count();
        cambiarEstado.Ejecutar(resultado.Id, "FINALIZADA");

        var orden = repo.ObtenerPorId(resultado.Id);
        Assert.Equal("FINALIZADA", orden!.Estado);
        var eventosNuevos = bitacora.ObtenerEventos().Skip(eventosPrevios);
        Assert.Contains(eventosNuevos, e => e.Contains($"Orden {resultado.Id} -> FINALIZADA"));
    }

    [Fact]
    public void Prueba07_CancelarPendiente()
    {
        var repo = new OrdenMemoryRepository();
        var bitacora = BitacoraSingleton.Instancia;
        var registrar = CrearRegistrarUseCase(repo, bitacora);
        var cambiarEstado = new CambiarEstadoOrdenUseCase(repo, repo, bitacora);

        var resultado = registrar.Ejecutar(new CrearOrdenDto("Ana", "LAPTOP", "Falla", "DIAGNOSTICO", "BASICO", "EMAIL", false, new List<string>()));
        int eventosPrevios = bitacora.ObtenerEventos().Count();
        cambiarEstado.Ejecutar(resultado.Id, "CANCELADA");

        var orden = repo.ObtenerPorId(resultado.Id);
        Assert.Equal("CANCELADA", orden!.Estado);
        var eventosNuevos = bitacora.ObtenerEventos().Skip(eventosPrevios);
        Assert.Contains(eventosNuevos, e => e.Contains($"Orden {resultado.Id} -> CANCELADA"));
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
    public void Prueba11b_ConstructorPlantilla_CopiaProfunda_NoAliasaListaDelLlamador()
    {
        var listaOriginal = new List<string> { "Tarea inicial" };
        var plantilla = new PlantillaReparacion("DIAGNOSTICO", listaOriginal);

        listaOriginal.Add("Tarea agregada despues de construir la plantilla");

        Assert.Single(plantilla.Tareas);
        Assert.DoesNotContain("Tarea agregada despues de construir la plantilla", plantilla.Tareas);
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

    [Fact]
    public void Prueba17_DI_RepositorioYBitacora_ResuelvenLaMismaInstanciaPorAmbasInterfaces()
    {
        // Reproduce el registro real de ReparaLab.WinForms/Program.cs para probar,
        // a traves de un contenedor de DI real, que IOrdenLecturaRepository e
        // IOrdenEscrituraRepository devuelven la MISMA instancia (el requisito de
        // CQRS "una sola clase, un solo singleton, expuesta por dos interfaces").
        var services = new ServiceCollection();
        services.AddSingleton<OrdenMemoryRepository>();
        services.AddSingleton<IOrdenLecturaRepository>(sp => sp.GetRequiredService<OrdenMemoryRepository>());
        services.AddSingleton<IOrdenEscrituraRepository>(sp => sp.GetRequiredService<OrdenMemoryRepository>());
        services.AddSingleton<IBitacoraService>(BitacoraSingleton.Instancia);

        using var proveedor = services.BuildServiceProvider();

        var lectura = proveedor.GetRequiredService<IOrdenLecturaRepository>();
        var escritura = proveedor.GetRequiredService<IOrdenEscrituraRepository>();
        var concreto = proveedor.GetRequiredService<OrdenMemoryRepository>();

        Assert.True(ReferenceEquals(lectura, escritura));
        Assert.True(ReferenceEquals(lectura, concreto));

        var bitacoraA = proveedor.GetRequiredService<IBitacoraService>();
        var bitacoraB = proveedor.GetRequiredService<IBitacoraService>();
        Assert.True(ReferenceEquals(bitacoraA, bitacoraB));
        Assert.True(ReferenceEquals(bitacoraA, BitacoraSingleton.Instancia));
    }

    [Fact]
    public void Prueba18_PlantillaCargadaDeOtroServicio_Rechazada()
    {
        var repo = new OrdenMemoryRepository();
        var registrar = CrearRegistrarUseCase(repo, BitacoraSingleton.Instancia);
        var dto = new CrearOrdenDto("Ana", "LAPTOP", "Falla", "MANTENIMIENTO", "BASICO", "EMAIL", false,
            new List<string> { "Revisión de hardware" }, "DIAGNOSTICO");

        var ex = Assert.Throws<ArgumentException>(() => registrar.Ejecutar(dto));

        Assert.Equal("La plantilla cargada no corresponde al servicio seleccionado.", ex.Message);
        Assert.Empty(repo.ObtenerTodas());
    }

    [Fact]
    public void Prueba19_BitacoraListaLasTareasDeLaPlantilla()
    {
        var repo = new OrdenMemoryRepository();
        var bitacora = BitacoraSingleton.Instancia;
        var registrar = CrearRegistrarUseCase(repo, bitacora);
        var plantilla = new ObtenerPlantillaUseCase(new CatalogoPlantillas()).Ejecutar("DIAGNOSTICO");
        int eventosPrevios = bitacora.ObtenerEventos().Count();

        registrar.Ejecutar(new CrearOrdenDto("Ana", "LAPTOP", "Falla", "DIAGNOSTICO", "BASICO", "EMAIL", false,
            plantilla.Tareas, plantilla.Servicio));

        var nuevos = bitacora.ObtenerEventos().Skip(eventosPrevios).ToList();
        foreach (var tarea in plantilla.Tareas)
            Assert.Contains(nuevos, e => e.Contains(tarea));
    }

    [Fact]
    public void Prueba20_CatalogoNoExponeLaPlantillaOriginalModificable()
    {
        var catalogo = new CatalogoPlantillas();

        catalogo.Obtener("DIAGNOSTICO").Tareas.Add("Intrusa");
        catalogo.Obtener("DIAGNOSTICO").NombreServicio = "OTRO";

        var otra = catalogo.Obtener("DIAGNOSTICO");
        Assert.DoesNotContain("Intrusa", otra.Tareas);
        Assert.Equal("DIAGNOSTICO", otra.NombreServicio);
    }
}
