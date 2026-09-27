using System.Drawing;

namespace ReparaLab.Base;

// PROYECTO BASE: funciona, pero mezcla interfaz, validaciones, precios,
// estados, almacenamiento y notificaciones. La refactorizacion es el APE 02.
public class FrmReparaciones : Form
{
    private readonly TextBox txtCliente = new();
    private readonly TextBox txtFalla = new();
    private readonly ComboBox cmbEquipo = new();
    private readonly ComboBox cmbServicio = new();
    private readonly ComboBox cmbPlan = new();
    private readonly ComboBox cmbNotificacion = new();
    private readonly CheckBox chkRepuesto = new();
    private readonly DataGridView tabla = new();
    private readonly TextBox txtEventos = new();
    private readonly List<OrdenReparacion> lista = new();
    private int siguienteId = 1;

    public FrmReparaciones()
    {
        Text = "ReparaLab - Proyecto base APE 02";
        Width = 1040;
        Height = 740;
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(970, 690);
        Font = new Font("Segoe UI", 10);
        ConstruirInterfaz();
        ActualizarTabla();
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

    private void AgregarEtiqueta(string texto, int x, int y) => Controls.Add(new Label { Text = texto, Left = x, Top = y, Width = 320, Height = 24 });

    private void PrepararCombo(ComboBox c, int x, int y, params string[] opciones)
    {
        c.SetBounds(x, y, 300, 32);
        c.DropDownStyle = ComboBoxStyle.DropDownList;
        c.Items.AddRange(opciones);
        c.SelectedIndex = 0;
        Controls.Add(c);
    }

    private Button CrearBoton(string texto, int x, int y, int ancho)
    {
        var btn = new Button { Text = texto, Left = x, Top = y, Width = ancho, Height = 35 };
        Controls.Add(btn);
        return btn;
    }

    private void Registrar()
    {
        // Mezcla intencional: esta funcion decide reglas, calcula,
        // construye entidades, guarda y simula notificacion.
        if (string.IsNullOrWhiteSpace(txtCliente.Text))
        {
            MessageBox.Show("Cliente obligatorio.");
            txtCliente.Focus();
            return;
        }
        if (string.IsNullOrWhiteSpace(txtFalla.Text))
        {
            MessageBox.Show("Describa la falla.");
            txtFalla.Focus();
            return;
        }
        if (cmbEquipo.SelectedItem == null || cmbServicio.SelectedItem == null || cmbPlan.SelectedItem == null || cmbNotificacion.SelectedItem == null)
        {
            MessageBox.Show("Seleccione todas las opciones.");
            return;
        }

        string servicio = cmbServicio.Text;
        string plan = cmbPlan.Text;
        decimal total = servicio == "DIAGNOSTICO" ? 20m : 35m;
        if (plan == "PREMIUM") total = total * 1.25m;
        if (chkRepuesto.Checked) total = total + 15m;
        int garantia = plan == "PREMIUM" ? 90 : 30;

        var orden = new OrdenReparacion
        {
            Id = siguienteId++, Cliente = txtCliente.Text.Trim(), Equipo = cmbEquipo.Text,
            Falla = txtFalla.Text.Trim(), Servicio = servicio, Plan = plan,
            Notificacion = cmbNotificacion.Text, Repuesto = chkRepuesto.Checked,
            Total = total, GarantiaDias = garantia, Estado = "PENDIENTE"
        };
        lista.Add(orden);
        ActualizarTabla();
        if (cmbNotificacion.Text == "EMAIL")
            AgregarEvento("EMAIL simulado: orden " + orden.Id + " registrada para " + orden.Cliente);
        else if (cmbNotificacion.Text == "SMS")
            AgregarEvento("SMS simulado: orden " + orden.Id + " registrada para " + orden.Cliente);
        AgregarEvento("Orden " + orden.Id + " guardada. Total $" + orden.Total.ToString("0.00") + ". Garantía " + orden.GarantiaDias + " días.");
        Limpiar();
    }

    private void CambiarEstado(string estado)
    {
        if (tabla.CurrentRow == null || !int.TryParse(tabla.CurrentRow.Cells["Id"].Value?.ToString(), out int id))
        {
            MessageBox.Show("Seleccione una orden de la tabla.");
            return;
        }
        OrdenReparacion? orden = lista.FirstOrDefault(x => x.Id == id);
        if (orden == null) { MessageBox.Show("Orden no encontrada."); return; }
        if (orden.Estado != "PENDIENTE")
        {
            MessageBox.Show("Solo se puede modificar una orden PENDIENTE.");
            return;
        }
        orden.Estado = estado;
        ActualizarTabla();
        AgregarEvento("Orden " + id + " -> " + estado);
    }

    private void ActualizarTabla()
    {
        tabla.DataSource = null;
        tabla.DataSource = lista.Select(x => new
        {
            x.Id, x.Cliente, x.Equipo, x.Servicio, x.Plan,
            x.Total, x.GarantiaDias, x.Estado
        }).ToList();
        if (tabla.Columns["GarantiaDias"] != null) tabla.Columns["GarantiaDias"].HeaderText = "Garantía (días)";
    }

    private void AgregarEvento(string texto) => txtEventos.AppendText(DateTime.Now.ToString("HH:mm:ss") + " | " + texto + Environment.NewLine);

    private void Limpiar()
    {
        txtCliente.Clear(); txtFalla.Clear();
        cmbEquipo.SelectedIndex = 0; cmbServicio.SelectedIndex = 0;
        cmbPlan.SelectedIndex = 0; cmbNotificacion.SelectedIndex = 0;
        chkRepuesto.Checked = false;
        txtCliente.Focus();
    }
}
