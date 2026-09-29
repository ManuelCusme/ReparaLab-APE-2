namespace ReparaLab.WinForms;

using Microsoft.Extensions.DependencyInjection;
using ReparaLab.Application.Interfaces;
using ReparaLab.Application.UseCases;
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
        Application.Run(mainForm);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Infraestructura (Singleton para mantener vivos los datos durante la sesión)
        services.AddSingleton<IOrdenRepository, OrdenMemoryRepository>();
        services.AddSingleton<IBitacoraService>(BitacoraSingleton.Instancia);

        // Casos de Uso
        services.AddTransient<RegistrarOrdenUseCase>();
        services.AddTransient<ListarOrdenesUseCase>();
        services.AddTransient<CambiarEstadoOrdenUseCase>();

        // Formulario Principal
        services.AddTransient<FrmReparaciones>();
    }
}