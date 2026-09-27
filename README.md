# ReparaLab.Base — proyecto inicial para el APE 02
JV

## Requisitos
- Windows 10/11 y Visual Studio 2022 con la carga de trabajo «Desarrollo de escritorio con .NET» y SDK de .NET 8.
- También se puede iniciar desde una terminal de Windows con el SDK instalado.

## Cómo ejecutar el proyecto inicial
1. Descomprimir el ZIP.
2. Abrir `ReparaLab.Base.csproj` en Visual Studio.
3. Ejecutar con F5 o Ctrl+F5. Alternativamente, abrir una terminal en la carpeta del proyecto y ejecutar `dotnet run`.
4. El formulario se construye mediante código C# en `FrmReparaciones.cs`; por eso se visualiza al **ejecutar** la aplicación y no depende de un archivo `.Designer.cs`.

## Funciones iniciales
- Registrar y listar órdenes de LAPTOP, CELULAR o TABLET.
- Seleccionar DIAGNOSTICO ($20) o MANTENIMIENTO ($35).
- Plan BASICO: tarifa original y garantía de 30 días.
- Plan PREMIUM: recargo de 25 % sobre el servicio y garantía de 90 días.
- Repuesto opcional: $15 añadidos después del recargo del plan.
- Notificación EMAIL o SMS **simulada** en el panel de eventos.
- Estado inicial PENDIENTE; una orden pendiente puede finalizarse o cancelarse. No se permiten nuevos cambios desde estados finales.
- Ejemplos: Ana / DIAGNOSTICO / BASICO / sin repuesto = $20.00; Luis / MANTENIMIENTO / PREMIUM / con repuesto = $58.75.


No se utiliza base de datos, Entity Framework, API REST, archivos para persistir órdenes ni envío real de mensajes. Los registros se pierden al cerrar la aplicación.
