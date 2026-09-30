# ReparaLab

Aplicacion de escritorio para gestionar ordenes de reparacion usando C# y .NET 8 Windows Forms. El trabajo refactoriza el proyecto inicial `ReparaLab.Base` aplicando Clean Architecture, SOLID y cinco patrones creacionales.

## Proyectos

- `ReparaLab.Domain`: entidad, reglas de estado, contratos, Builder, Abstract Factory y Prototype.
- `ReparaLab.Application`: DTOs, casos de uso y coordinacion de los flujos.
- `ReparaLab.Infrastructure`: repositorio en memoria, notificaciones simuladas y bitacora Singleton.
- `ReparaLab.WinForms`: interfaz grafica y composition root con inyeccion de dependencias.
- `ReparaLab.Tests`: pruebas automatizadas de la solucion.
- `ReparaLab.Base`: proyecto inicial utilizado como referencia del ANTES.

## Funcionalidad

- Registro y listado de ordenes.
- Equipos: LAPTOP, CELULAR y TABLET.
- Servicios: DIAGNOSTICO ($20) y MANTENIMIENTO ($35).
- Plan BASICO: tarifa base y garantia de 30 dias.
- Plan PREMIUM: 25% adicional sobre el servicio y garantia de 90 dias.
- Repuesto opcional de $15 despues de aplicar el plan.
- Notificaciones EMAIL y SMS simuladas.
- Estados PENDIENTE, FINALIZADA y CANCELADA.
- Solo se permiten transiciones desde PENDIENTE.
- Plantillas clonables para DIAGNOSTICO y MANTENIMIENTO.
- Datos almacenados exclusivamente en memoria durante la sesion.

## Patrones aplicados

- **Factory Method**: crea notificadores EMAIL y SMS mediante fabricas concretas.
- **Abstract Factory**: crea familias coherentes de politicas de tarifa y garantia para BASICO y PREMIUM.
- **Builder**: construye `OrdenReparacion` paso a paso mediante una fabrica inyectada.
- **Prototype**: clona plantillas con copia independiente de la lista de tareas.
- **Singleton**: mantiene una bitacora compartida durante la ejecucion.

## Arquitectura y flujo

```text
WinForms -> Application -> Domain
    |             |
    +--------> Infrastructure
```

Flujo de registro:

```text
FrmReparaciones
  -> RegistrarOrdenUseCase
  -> Abstract Factory
  -> Builder
  -> IOrdenEscrituraRepository
  -> Factory Method
  -> IBitacoraService
  -> resultado visual
```

La lectura y escritura del repositorio se separan mediante `IOrdenLecturaRepository` e `IOrdenEscrituraRepository`. Ambas interfaces usan la misma instancia Singleton de `OrdenMemoryRepository` durante la sesion.

## Requisitos

- Windows 10/11.
- SDK de .NET 8.
- Windows Forms.

No se utiliza base de datos, Entity Framework, API REST, archivos para persistir ordenes ni envio real de mensajes. Los datos se pierden al cerrar la aplicacion.

## Ejecucion

Desde la raiz del repositorio:

```powershell
dotnet restore
dotnet build ReparaLab.sln
dotnet run --project ReparaLab.WinForms/ReparaLab.WinForms.csproj
```

