namespace ReparaLab.WinForms;

using System.Drawing;
using ReparaLab.Application.DTOs;
using ReparaLab.Application.UseCases;
using ReparaLab.Domain;

public class FrmReparaciones : Form
{
    private readonly RegistrarOrdenUseCase _registrarUseCase;
    private readonly ListarOrdenesUseCase _listarUseCase;
    private readonly CambiarEstadoOrdenUseCase _cambiarEstadoUseCase;
    private readonly ObtenerPlantillaUseCase _obtenerPlantillaUseCase;
    private readonly IBitacoraService _bitacora;

    private readonly TextBox txtCliente = new();
    private readonly TextBox txtFalla = new();
    private readonly ComboBox cmbEquipo = new();
    private readonly ComboBox cmbServicio = new();
    private readonly ComboBox cmbPlan = new();
    private readonly ComboBox cmbNotificacion = new();
    private readonly CheckBox chkRepuesto = new();
    private readonly ComboBox cmbPlantilla = new();
    private readonly TextBox txtTareasPlantilla = new();
    private string? _plantillaCargada;
    private readonly DataGridView tabla = new();
    private readonly TextBox txtEventos = new();

    public FrmReparaciones(
        RegistrarOrdenUseCase registrarUseCase,
        ListarOrdenesUseCase listarUseCase,
        CambiarEstadoOrdenUseCase cambiarEstadoUseCase,
        ObtenerPlantillaUseCase obtenerPlantillaUseCase,
        IBitacoraService bitacora)
    {
        _registrarUseCase = registrarUseCase;
        _listarUseCase = listarUseCase;
        _cambiarEstadoUseCase = cambiarEstadoUseCase;
        _obtenerPlantillaUseCase = obtenerPlantillaUseCase;
        _bitacora = bitacora;

        Text = "ReparaLab - Proyecto base APE 02";
        Width = 1040; Height = 740;
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(970, 690);
        Font = new Font("Segoe UI", 10);

        ConstruirInterfaz();
        ActualizarTabla();
        ActualizarBitacoraView();
    }

    private void ConstruirInterfaz()
    {
        var titulo = new Label { Text = "ReparaLab | Órdenes de reparación", Left = 20, Top = 15, Width = 920, Height = 35, Font = new Font("Segoe UI", 16, FontStyle.Bold) };
        Controls.Add(titulo);

        AgregarEtiqueta("Cliente", 20, 65);
        txtCliente.SetBounds(20, 90, 300, 30);
        Controls.Add(txtCliente);

        AgregarEtiqueta("Equipo", 345, 65);
        PrepararCombo(cmbEquipo, 345, 90, "LAPTOP", "CELULAR", "TABLET");

        AgregarEtiqueta("Falla reportada", 670, 65);
        txtFalla.SetBounds(670, 90, 320, 30);
        Controls.Add(txtFalla);

        AgregarEtiqueta("Servicio", 20, 140);
        PrepararCombo(cmbServicio, 20, 165, "DIAGNOSTICO", "MANTENIMIENTO");

        AgregarEtiqueta("Plan", 345, 140);
        PrepararCombo(cmbPlan, 345, 165, "BASICO", "PREMIUM");

        AgregarEtiqueta("Aviso simulado", 670, 140);
        PrepararCombo(cmbNotificacion, 670, 165, "EMAIL", "SMS");

        chkRepuesto.Text = "Agregar repuesto fijo ($15)";
        chkRepuesto.SetBounds(20, 212, 300, 30);
        Controls.Add(chkRepuesto);

        // Plantilla (Prototype): agregado minimo en el espacio libre de la fila del checkbox
        cmbPlantilla.SetBounds(345, 212, 160, 30);
        cmbPlantilla.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPlantilla.Items.AddRange(new object[] { "Ninguna", "DIAGNOSTICO", "MANTENIMIENTO" });
        cmbPlantilla.SelectedIndex = 0;
        Controls.Add(cmbPlantilla);

        var btnCargarPlantilla = CrearBoton("Cargar plantilla", 515, 210, 140);
        btnCargarPlantilla.Click += (_, _) => CargarPlantilla();

        txtTareasPlantilla.SetBounds(665, 212, 325, 30);
        Controls.Add(txtTareasPlantilla);

        var btnRegistrar = CrearBoton("Registrar orden", 20, 260, 180);
        btnRegistrar.Click += (_, _) => Registrar();

        var btnFinalizar = CrearBoton("Finalizar seleccionada", 215, 260, 205);
        btnFinalizar.Click += (_, _) => CambiarEstado("FINALIZADA");

        var btnCancelar = CrearBoton("Cancelar seleccionada", 435, 260, 200);
        btnCancelar.Click += (_, _) => CambiarEstado("CANCELADA");

        var btnLimpiar = CrearBoton("Limpiar", 650, 260, 130);
        btnLimpiar.Click += (_, _) => Limpiar();

        tabla.SetBounds(20, 310, 970, 230);
        tabla.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        tabla.ReadOnly = true;
        tabla.AllowUserToAddRows = false;
        tabla.AllowUserToDeleteRows = false;
        tabla.MultiSelect = false;
        tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        Controls.Add(tabla);

        AgregarEtiqueta("Eventos y notificaciones simuladas (sin correo ni SMS reales)", 20, 550);
        txtEventos.SetBounds(20, 576, 970, 95);
        txtEventos.Multiline = true;
        txtEventos.ReadOnly = true;
        txtEventos.ScrollBars = ScrollBars.Vertical;
        txtEventos.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        Controls.Add(txtEventos);
    }

    private void Registrar()
    {
        try
        {
            var tareas = string.IsNullOrWhiteSpace(txtTareasPlantilla.Text)
                ? new List<string>()
                : txtTareasPlantilla.Text
                    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToList();

            var dto = new CrearOrdenDto(
                txtCliente.Text.Trim(),
                cmbEquipo.Text,
                txtFalla.Text.Trim(),
                cmbServicio.Text,
                cmbPlan.Text,
                cmbNotificacion.Text,
                chkRepuesto.Checked,
                tareas,
                _plantillaCargada
            );

            int eventosPrevios = _bitacora.ObtenerEventos().Count();
            var resultado = _registrarUseCase.Ejecutar(dto);
            ActualizarTabla();
            ActualizarBitacoraView();

            var avisos = _bitacora.ObtenerEventos().Skip(eventosPrevios);
            MessageBox.Show(
                $"Orden {resultado.Id} registrada.\nTotal: {resultado.Total:0.00}\nGarantía: {resultado.GarantiaDias} días\n\n{string.Join("\n", avisos)}",
                "Orden registrada", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Limpiar();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Validación o Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void CambiarEstado(string nuevoEstado)
    {
        if (tabla.CurrentRow == null || !int.TryParse(tabla.CurrentRow.Cells["Id"].Value?.ToString(), out int id))
        {
            MessageBox.Show("Seleccione una orden de la tabla.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            _cambiarEstadoUseCase.Ejecutar(id, nuevoEstado);
            ActualizarTabla();
            ActualizarBitacoraView();
            MessageBox.Show(
                $"La orden {id} pasó a estado {nuevoEstado}.",
                "Estado actualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Operación no permitida", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CargarPlantilla()
    {
        if (cmbPlantilla.SelectedIndex <= 0)
        {
            txtTareasPlantilla.Clear();
            _plantillaCargada = null;
            return;
        }

        try
        {
            var plantilla = _obtenerPlantillaUseCase.Ejecutar(cmbPlantilla.Text);
            cmbServicio.Text = plantilla.Servicio;
            txtTareasPlantilla.Text = string.Join("; ", plantilla.Tareas);
            _plantillaCargada = plantilla.Servicio;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Plantilla", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void ActualizarTabla()
    {
        tabla.DataSource = null;
        tabla.DataSource = _listarUseCase.Ejecutar().ToList();
        if (tabla.Columns["GarantiaDias"] != null)
            tabla.Columns["GarantiaDias"].HeaderText = "Garantía (días)";
        if (tabla.Columns["Total"] != null)
            tabla.Columns["Total"].DefaultCellStyle.Format = "0.00";
    }

    private void ActualizarBitacoraView()
    {
        txtEventos.Lines = _bitacora.ObtenerEventos().ToArray();
    }

    private void AgregarEtiqueta(string texto, int x, int y) => Controls.Add(new Label { Text = texto, Left = x, Top = y, Width = 320, Height = 24 });
    private void PrepararCombo(ComboBox c, int x, int y, params string[] opciones) { c.SetBounds(x, y, 300, 32); c.DropDownStyle = ComboBoxStyle.DropDownList; c.Items.AddRange(opciones); c.SelectedIndex = 0; Controls.Add(c); }
    private Button CrearBoton(string texto, int x, int y, int ancho) { var btn = new Button { Text = texto, Left = x, Top = y, Width = ancho, Height = 35 }; Controls.Add(btn); return btn; }
    private void Limpiar()
    {
        txtCliente.Clear(); txtFalla.Clear();
        cmbEquipo.SelectedIndex = 0; cmbServicio.SelectedIndex = 0;
        cmbPlan.SelectedIndex = 0; cmbNotificacion.SelectedIndex = 0;
        chkRepuesto.Checked = false;
        cmbPlantilla.SelectedIndex = 0;
        txtTareasPlantilla.Clear();
        _plantillaCargada = null;
        txtCliente.Focus();
    }
}
