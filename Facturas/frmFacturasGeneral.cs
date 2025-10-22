using Logica;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace ModuloCajaRC.Facturas
{
    public partial class frmFacturasGeneral : Form
    {
        DataTable dtPermisos = new DataTable();
        DataTable dtFacturas = new DataTable();
        DataTable dtFacturasEncabezado = new DataTable();
        DataTable dtFacturasMetodos = new DataTable();
        DataTable dtMetodoPago = new DataTable();
        clsLogica logica = new clsLogica();

        public int _FacturaID;
        public string _GuiaID;
        private bool usuarioInteractuando = false;
        public bool pasaValidacion = true;
        private bool _GeneraError = true;
        private bool _ErrorRecibido = true;

        Label lblTooltipFlotante = new Label
        {
            AutoSize = true,
            BackColor = Color.LightYellow,
            ForeColor = Color.Black,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Visible = false
        };
        public frmFacturasGeneral()
        {
            InitializeComponent();
            CargarAcciones(1, pAcciones, 60, 50, 10, TextImageRelation.ImageAboveText);
            CargarFacturas();
            CargarMetodoPago();
        }

        private void frmFacturasGeneral_Load(object sender, EventArgs e)
        {
            tmrBuscarFacturas.Start();
            this.Controls.Add(lblTooltipFlotante);
        }
        private void CargarFacturas()
        {
            FacturaProcesoDTO getFacturas = new FacturaProcesoDTO
            {
                Opcion = "RECUPERAR",
                Proceso = 1
            };

            DataTable nuevaTabla = logica.SP_FacturasProceso(getFacturas);

            if (nuevaTabla.Rows.Count == 0)
                return;

            // Validamos si hay cambios reales en el ProcesoID
            string nuevoID = nuevaTabla.Rows[0]["ProcesoID"].ToString();
            string actualID = dgvFacturas.Rows.Count > 0 && dgvFacturas.Rows[0].Cells["ProcesoID"].Value != null
                ? dgvFacturas.Rows[0].Cells["ProcesoID"].Value.ToString()
                : "";

            if (nuevoID != actualID)
            {
                // Guardamos la fila seleccionada
                int filaActual = dgvFacturas.CurrentRow?.Index ?? -1;

                // Solo actualizamos el DataSource si hay cambios
                dgvFacturas.SuspendLayout(); // Pausa el redibujo
                dgvFacturas.DataSource = nuevaTabla;
                dgvFacturas.ResumeLayout(); // Reactiva el redibujo

                // Restauramos la selección si es válida
                if (filaActual >= 0 && filaActual < dgvFacturas.Rows.Count)
                {
                    dgvFacturas.CurrentCell = dgvFacturas.Rows[filaActual].Cells["Total"];
                    lblGranTotal.Text = dgvFacturas.Rows[filaActual].Cells["Total"].Value?.ToString() ?? "L. 0.00";
                }
                else
                {
                    dgvFacturas.CurrentCell = dgvFacturas.Rows[0].Cells["Total"];
                    lblGranTotal.Text = dgvFacturas.Rows[0].Cells["Total"].Value?.ToString() ?? "L. 0.00";
                }
            }

            dgvFacturas.Columns["ProcesoID"].Visible = false;
            dgvFacturas.Columns["Fecha"].Visible = false;
            dgvFacturas.Columns["Fila"].Visible = false;
            dgvFacturas.Columns["Proceso"].Visible = false;
            dgvFacturas.Columns["Destinatario"].Visible = false;
            dgvFacturas.Columns["FacturaDei"].Visible = false;

            dgvFacturas.Columns["Remitente"].Width = 150;
            dgvFacturas.Columns["FacturaID"].Width = 100;
            dgvFacturas.Columns["Guia"].Width = 100;
            dgvFacturas.Columns["Total"].Width = 100;

            CargarResumen();
        }

        private void CargarMetodoPago()
        {
            MetodoPagoDTO getMetodo = new MetodoPagoDTO
            {
                Opcion = "LISTADO",
            };
            dtMetodoPago = logica.SP_MetodoPagos(getMetodo);
            if (dtMetodoPago.Rows.Count > 0)
            {
                foreach (DataRow row in dtMetodoPago.Rows)
                {
                    dgvMetodosPago.Rows.Add(row["MetodoID"], row["Descripcion"], "","");

                    dgvMetodosPago.Columns["Id"].Visible = false;
                    dgvMetodosPago.Columns["Metodo"].Width = 150;
                    dgvMetodosPago.Columns["Valor"].Width = 100;
                    dgvMetodosPago.Columns["Referencia"].Width = 150;
                    dgvMetodosPago.Columns["Ayuda"].Width = 35;

                    dgvMetodosPago.Columns["Ayuda"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvMetodosPago.Columns["Ayuda"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
        }

        public void CargarAcciones(int _UbicacionID, FlowLayoutPanel _Panel, int _Ancho, int _Alto, int _Padding, TextImageRelation _Relacion)
        {
            dtPermisos.Clear();
            TBLPermisosEspecificos getPermisos = new TBLPermisosEspecificos()
            {
                Opcion = "ListadoPorFormulario",
                UsuarioID = DynamicMain.usuarioIDNumber,
                NombreFormulario = this.Name,
                UbicacionID = _UbicacionID,
                ModuloID = DynamicMain.ModuloID
            };
            dtPermisos = logica.SP_PermisosEspecificos(getPermisos);
            if (dtPermisos.Rows.Count > 0)
            {
                foreach (DataRow row in dtPermisos.Rows)
                {
                    string nombreElemento = row["NombreElemento"].ToString();
                    string accionElemento = row["AccionElemento"].ToString();
                    string iconoElemento = row["IconoElemento"] != DBNull.Value ? row["IconoElemento"].ToString() : null;
                    bool visible = Convert.ToBoolean(row["Visible"]);

                    if (visible)
                    {
                        Button btn = new Button();
                        btn.Name = nombreElemento;
                        btn.Text = accionElemento;
                        btn.Width = _Ancho;
                        btn.Height = _Alto;
                        btn.Margin = new Padding(_Padding);

                        if (!string.IsNullOrEmpty(iconoElemento))
                        {
                            try
                            {
                                var resourceManager = ModuloCajaRC.Properties.Resources.ResourceManager;
                                object icono = resourceManager.GetObject(Path.GetFileNameWithoutExtension(iconoElemento));

                                if (icono != null && icono is Image)
                                {
                                    btn.Image = new Bitmap((Image)icono, new Size(24, 24));
                                    btn.TextAlign = ContentAlignment.MiddleCenter;
                                    btn.ImageAlign = ContentAlignment.BottomCenter;
                                    btn.TextImageRelation = _Relacion;
                                }
                                else
                                {
                                    btn.TextAlign = ContentAlignment.MiddleCenter;
                                }
                            }
                            catch
                            {
                                btn.TextAlign = ContentAlignment.MiddleCenter;
                            }
                        }
                        else
                        {
                            btn.TextAlign = ContentAlignment.MiddleCenter;
                        }

                        btn.Click += (s, e) =>
                        {
                            switch (accionElemento)
                            {
                                case "Procesar":
                                    InsertarCobro();
                                    
                                    dgvFacturas.DataSource = null;
                                    CargarFacturas();
                                    break;

                                case "Limpiar":
                                    Limpiar();
                                    dgvFacturas.DataSource = null;
                                    break;
                                default:
                                    MessageBox.Show($"Acción no reconocida: {accionElemento}");
                                    break;
                            }
                        };

                        _Panel.Controls.Add(btn);
                        _Panel.WrapContents = true;
                    }
                }
            }
        }
        private void Validaciones()
        {
            pasaValidacion = true;
            _GeneraError = true;
            _ErrorRecibido = true;

            decimal totalValor = 0;

            foreach (DataGridViewRow row in dgvMetodosPago.Rows)
            {
                if (row.Cells["Valor"]?.Value != null && decimal.TryParse(row.Cells["Valor"].Value.ToString(), out decimal valor))
                {
                    totalValor += valor;
                }
            }
            _GeneraError = (totalValor > 0) ? true : false;
            pasaValidacion = true;

            foreach (DataGridViewRow row in dgvMetodosPago.Rows)
            {
                var valorObj = row.Cells["Valor"]?.Value;

                if (valorObj != null && !string.IsNullOrWhiteSpace(valorObj.ToString()))
                {
                    int valor;
                    if (int.TryParse(valorObj.ToString(), out valor))
                    {
                        int metodoID = Convert.ToInt32(row.Cells["ID"].Value);
                        var referenciaObj = row.Cells["Referencia"]?.Value;
                        string referencia = referenciaObj != null ? referenciaObj.ToString() : "";

                        if ((metodoID == 3 || metodoID == 5 || metodoID == 6) && string.IsNullOrWhiteSpace(referencia))
                        {
                            pasaValidacion = false;
                            break; // Opcional: salir del loop si ya no pasa
                        }
                    }
                }
            }
          //Valido que el total recibido no sea menor al total esperado
            _ErrorRecibido = (Convert.ToDecimal(lblRecibido.Text) < Convert.ToDecimal(lblGranTotal.Text)) ? true : false;

        }

        private void InsertarCobro() 
        {
            Validaciones();
            if (_ErrorRecibido == true)
            {
                MessageBox.Show("El total recibido no puede ser menor al total esperado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (pasaValidacion == false) {
                MessageBox.Show("Debe completar el campo de referencia cuando no sea efectivo.","Aviso",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(_GeneraError == false) { return; }


            CobroCajaEncabezadoDTO sendEncabezado = new CobroCajaEncabezadoDTO
            {
                Opcion = "Agregar",
                ControlCajaID = DynamicMain.cajaID,
                FacturaID = _FacturaID,
                TotalAPagar = Convert.ToDecimal(lblGranTotal.Text),
                TotalRecibido = Convert.ToDecimal(lblRecibido.Text),
                TotalCambio = Convert.ToDecimal(lblCambio.Text),
                UPosteo = DynamicMain.usuarionlogin,
                FPosteo = DateTime.Now,
                PC = System.Environment.MachineName,
                Estado = true
            };
            dtFacturasEncabezado = logica.SP_CobroCajaEncabezado(sendEncabezado);
            if (dtFacturasEncabezado.Rows.Count > 0 && dtFacturasEncabezado.Rows[0]["Estado"].ToString() == "1")
            {
                int _EncabezadoID = Convert.ToInt32(dtFacturasEncabezado.Rows[0]["UltimoID"]);

                foreach (DataGridViewRow row in dgvMetodosPago.Rows)
                {
                    var valorCell = row.Cells["Valor"]?.Value;

                    // Validar si la celda está vacía, nula o no convertible
                    if (valorCell == null || string.IsNullOrWhiteSpace(valorCell.ToString()))
                        continue;

                    //Validar que la celda tenga un valor valido y no otras cosas
                    if (!decimal.TryParse(valorCell.ToString(), out decimal monto))
                        continue;

                    if (Convert.ToInt32(row.Cells["Id"].Value) == 4 && Convert.ToDecimal (lblCambio.Text) > 0) 
                    {
                        monto = Convert.ToDecimal(row.Cells["Valor"]?.Value) - Convert.ToDecimal(lblCambio.Text);
                    }
                    
                    CobroCajaMetodosDTO sendMetodos = new CobroCajaMetodosDTO
                    {
                        Opcion = "Agregar",
                        EncabezadoID = _EncabezadoID,
                        MetodoPagoID = Convert.ToInt32(row.Cells["Id"].Value),
                        MontoIngresado = monto,
                        Referencia = row.Cells["Referencia"]?.Value?.ToString() ?? "",
                        UPosteo = DynamicMain.usuarionlogin,
                        FPosteo = DateTime.Now,
                        PC = System.Environment.MachineName,
                        Estado = true
                    };

                    dtFacturasMetodos = logica.SP_CobroCajaMetodos(sendMetodos);

                    if ((dtFacturasMetodos.Rows.Count > 0 && dtFacturasMetodos.Rows[0]["Estado"].ToString() == "0") || dtFacturasMetodos.Rows.Count <= 0)
                    {
                        MessageBox.Show(dtFacturasMetodos.Rows[0]["Mensaje"].ToString(), "Notificación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                ActualizarEstadoFactura();
                Limpiar();

            }
            else // Hubo clavo desde el encabezado
            {
                MessageBox.Show(dtFacturasMetodos.Rows[0]["Mensaje"].ToString(), "Notificación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void ActualizarEstadoFactura()
        {
            FacturaProcesoDTO sendFacturas = new FacturaProcesoDTO
            {
                Opcion = "AGREGAR",
                FacturaID = _FacturaID,
                Proceso = 2,
                UPosteo = DynamicMain.usuarionlogin,
                FPosteo = DateTime.Now,
                PC = System.Environment.MachineName,
                Guia = _GuiaID
            };
            dtFacturas = logica.SP_FacturasProceso(sendFacturas);
            if (dtFacturas.Rows.Count > 0 && dtFacturas.Rows[0]["Estado"].ToString() == "1")
            {
                MessageBox.Show("Registro Ingresado correctamente, Solicitud de busqueda de carga enviada a bodega.", "Notificación", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Ha ocurrido un error al intentar enviar a bodega", "Notificacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void tmrBuscarFacturas_Tick(object sender, EventArgs e)
        {
            if (!usuarioInteractuando)
            {
                CargarFacturas();
            }
        }

        private void dgvFacturas_Click(object sender, EventArgs e)
        {
            if (dgvFacturas.Rows.Count > 0)
            {
                Limpiar();
                lblGranTotal.Text = dgvFacturas.CurrentRow.Cells["Total"].Value.ToString();
            }
        }
        private void Limpiar()
        {
            foreach (DataGridViewRow row in dgvMetodosPago.Rows)
            {
                row.Cells["Valor"].Value = "";
                row.Cells["Referencia"].Value = "";
            }

            lblGranTotal.Text = "0.00";
            lblRecibido.Text = "0.00";
            lblCambio.Text = "0.00";
            lblGranTotal.Text = "0.00";
            lblGuia.Text = "-";
            lblFactura.Text = "-";
            lblFecha.Text = "-";
            lblRemitente.Text = "-";
            lblDestinatario.Text = "-";
            lblRecibido.Text = "0.00";
            _FacturaID = 0;
            _GuiaID = String.Empty;
        }

        private void dgvMetodosPago_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvMetodosPago.Columns[e.ColumnIndex].Name == "Valor")
            {
                var celda = dgvMetodosPago.Rows[e.RowIndex].Cells[e.ColumnIndex];
                string texto = celda.Value?.ToString() ?? "";

                if (!decimal.TryParse(texto, out decimal valor))
                {
                    celda.Value = DBNull.Value;
                }

                ActualizarTotalRecibido();
            }
        }
        private void ActualizarTotalRecibido()
        {
            decimal total = 0;

            foreach (DataGridViewRow fila in dgvMetodosPago.Rows)
            {
                if (fila.Cells["Valor"].Value != null &&
                    decimal.TryParse(fila.Cells["Valor"].Value.ToString(), out decimal valor))
                {
                    total += valor;
                }
            }
            lblCambio.Text = (total > Convert.ToDecimal(lblGranTotal.Text)) ? (total - Convert.ToDecimal(lblGranTotal.Text)).ToString() : "0.00";
            lblRestante.Text = (total < Convert.ToDecimal(lblGranTotal.Text)? (total - Convert.ToDecimal(lblGranTotal.Text)).ToString() : "0.00" );
            lblRecibido.Text = $"{total:N2}";
        }

        private void dgvFacturas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvFacturas.Rows.Count > 0)
            {
                usuarioInteractuando = true;

                if (dgvFacturas.CurrentRow.Cells["Total"].Value != null)
                {
                    CargarResumen();
                }

                Task.Delay(100).ContinueWith(_ =>
                {
                    usuarioInteractuando = false;
                });
            }
        }
        private void CargarResumen() 
        {
            if (dgvFacturas.CurrentRow.Cells["Total"].Value != null)
            {
                lblFecha.Text = Convert.ToDateTime(dgvFacturas.CurrentRow.Cells["Fecha"].Value).ToString("dd/MM/yyyy");
                lblRemitente.Text = dgvFacturas.CurrentRow.Cells["Remitente"].Value.ToString();
                lblDestinatario.Text = dgvFacturas.CurrentRow.Cells["Destinatario"].Value.ToString();
                lblGuia.Text = dgvFacturas.CurrentRow.Cells["Guia"].Value.ToString();
                lblFactura.Text = dgvFacturas.CurrentRow.Cells["FacturaDei"].Value.ToString();
                lblTotal.Text = dgvFacturas.CurrentRow.Cells["Total"].Value.ToString();
                lblGranTotal.Text = dgvFacturas.CurrentRow.Cells["Total"].Value.ToString();
                _FacturaID = Convert.ToInt32(dgvFacturas.CurrentRow.Cells["FacturaID"].Value);
                _GuiaID = dgvFacturas.CurrentRow.Cells["Guia"].Value.ToString();
            }
        }
        private async void dgvMetodosPago_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvMetodosPago.Columns[e.ColumnIndex].Name == "Ayuda")
            {
                var idValue = dgvMetodosPago.Rows[e.RowIndex].Cells["Id"].Value?.ToString();
                string mensaje = null;

                switch (idValue)
                {
                    case "3":
                        mensaje = "Número de referencia";
                        break;
                    case "4":
                        mensaje = "No Referencia";
                        break;
                    case "5":
                    case "6":
                        mensaje = "No. Transacción";
                        break;
                }

                if (!string.IsNullOrEmpty(mensaje))
                {
                    Rectangle celda = dgvMetodosPago.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                    Point posicion = dgvMetodosPago.PointToScreen(new Point(440, celda.Top));

                    lblTooltipFlotante.Text = mensaje;
                    lblTooltipFlotante.Location = this.PointToClient(posicion);
                    lblTooltipFlotante.BringToFront();
                    lblTooltipFlotante.Visible = true;

                    await Task.Delay(10000);
                    lblTooltipFlotante.Visible = false;
                }
            }
        }

    }
}
