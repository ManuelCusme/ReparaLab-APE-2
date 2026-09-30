namespace ReparaLab.WinForms;

using Microsoft.Extensions.DependencyInjection;
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

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var services = new ServiceCollection();
        ConfigureServices(services);

        using var serviceProvider = services.BuildServiceProvider();
        var mainForm = serviceProvider.GetRequiredService<FrmReparaciones>();
        System.Windows.Forms.Application.Run(mainForm);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Infraestructura (Singleton para mantener vivos los datos durante la sesión)
        services.AddSingleton<OrdenMemoryRepository>();
        services.AddSingleton<IOrdenLecturaRepository>(sp => sp.GetRequiredService<OrdenMemoryRepository>());
        services.AddSingleton<IOrdenEscrituraRepository>(sp => sp.GetRequiredService<OrdenMemoryRepository>());
        services.AddSingleton<IBitacoraService>(BitacoraSingleton.Instancia);
        services.AddSingleton<CatalogoPlantillas>();
        services.AddSingleton<PlanFactorySelector>();
        services.AddTransient<IOrdenReparacionBuilderFactory, OrdenReparacionBuilderFactory>();
        services.AddSingleton<INotificadorFactorySelector, NotificadorFactorySelector>();

        // Casos de Uso
        services.AddTransient<RegistrarOrdenUseCase>();
        services.AddTransient<ListarOrdenesUseCase>();
        services.AddTransient<CambiarEstadoOrdenUseCase>();
        services.AddTransient<ObtenerPlantillaUseCase>();

        // Formulario Principal
        services.AddTransient<FrmReparaciones>();
    }
}
