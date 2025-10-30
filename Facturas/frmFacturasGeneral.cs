using Logica;
using Microsoft.Reporting.WinForms;
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
        DataTable dtFacturaUpdateTasa = new DataTable();
        DataTable dtBodegaExistente = new DataTable();
        DataTable dtFacturasEncabezado = new DataTable();
        DataTable dtFacturasMetodos = new DataTable();
        DataTable dtMetodoPago = new DataTable();
        clsLogica logica = new clsLogica();

        public int _FacturaID, _IDCobroCajaEncabezado, _IDCobroMetodo, _IDControlCaja;
        public string _GuiaID;
        private bool usuarioInteractuando = false;
        public bool pasaValidacion = true;
        private bool _GeneraError = true;
        private bool _ErrorRecibido = true;
        private bool _CambioError = false;
        public frmFacturasGeneral()
        {
            InitializeComponent();
            CargarAcciones(1, pAcciones, 70, 50, 10, TextImageRelation.ImageAboveText);
            CargarFacturas();
            CargarMetodoPago();
        }

        private void frmFacturasGeneral_Load(object sender, EventArgs e)
        {
            tmrBuscarFacturas.Start();
        }
        private void CargarFacturas()
        {
            // Traigo la data del ENAC
            FacturaProcesoDTO getFacturas = new FacturaProcesoDTO
            {
                Opcion = "RECUPERAR",
                Proceso = 1
            };
            DataTable nuevaTabla = logica.SP_FacturasProceso(getFacturas);

            // Traigo la data del INTER
            FacturaProcesoDTO50 getFacturas50 = new FacturaProcesoDTO50
            {
                Opcion = "RECUPERAR",
                Proceso = 1
            };
            DataTable nuevaTabla50 = logica.SP_FacturasProceso50(getFacturas50);

            // Si ambas están vacías, no hacemos nada
            if (nuevaTabla.Rows.Count == 0 && nuevaTabla50.Rows.Count == 0)
                return;

            // Fusionamos ambas tablas en una sola
            DataTable tablaFusionada = nuevaTabla.Clone(); // Copiamos estructura
            foreach (DataRow row in nuevaTabla.Rows)
                tablaFusionada.ImportRow(row);
            foreach (DataRow row in nuevaTabla50.Rows)
                tablaFusionada.ImportRow(row);

            // Validamos si hay cambios reales en el ProcesoID
            string nuevoID = tablaFusionada.Rows[0]["ProcesoID"].ToString();
            string actualID = dgvFacturas.Rows.Count > 0 && dgvFacturas.Rows[0].Cells["ProcesoID"].Value != null
                ? dgvFacturas.Rows[0].Cells["ProcesoID"].Value.ToString()
                : "";

            if (nuevoID != actualID)
            {
                int filaActual = dgvFacturas.CurrentRow?.Index ?? -1;

                dgvFacturas.SuspendLayout();
                dgvFacturas.DataSource = tablaFusionada;
                dgvFacturas.ResumeLayout();

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

            // Configuración visual
            dgvFacturas.Columns["ProcesoID"].Visible = true;
            dgvFacturas.Columns["Fecha"].Visible = false;
            dgvFacturas.Columns["Fila"].Visible = false;
            dgvFacturas.Columns["Proceso"].Visible = false;
            dgvFacturas.Columns["Remitente"].Visible = false;
            dgvFacturas.Columns["FacturaDei"].Visible = false;

            dgvFacturas.Columns["Remitente"].Width = 150;
            dgvFacturas.Columns["FacturaID"].Width = 100;
            dgvFacturas.Columns["Guia"].Width = 100;
            dgvFacturas.Columns["Total"].Width = 100;

            CargarResumen();
        }


        private void CargarMetodoPago()
        {
            dgvMetodosPago.Rows.Clear();

            MetodoPagoDTO getMetodo = new MetodoPagoDTO
            {
                Opcion = "LISTADO",
            };
            dtMetodoPago = logica.SP_MetodoPagos(getMetodo);
            if (dtMetodoPago.Rows.Count > 0)
            {
                foreach (DataRow row in dtMetodoPago.Rows)
                {
                    dgvMetodosPago.Rows.Add(row["MetodoID"], row["Descripcion"], "", "");

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

                                case "Rechazar":
                                    RechazarFactura();

                                    dgvFacturas.DataSource = null;
                                    CargarFacturas();
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
        private void RechazarFactura()
        {
            DialogResult resultado = MessageBox.Show("¿Desea rechazar esta factura?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                if (dgvFacturas.CurrentRow.Cells["Origen"].Value.ToString() == "ENAC.")
                {
                    FacturaProcesoDTO sendFacturas = new FacturaProcesoDTO
                    {
                        Opcion = "ELIMINAR",
                        FacturaID = _FacturaID,
                    };
                    dtFacturas = logica.SP_FacturasProceso(sendFacturas);
                }
                else if (dgvFacturas.CurrentRow.Cells["Origen"].Value.ToString() == "INTER")
                {
                    FacturaProcesoDTO50 sendFacturas = new FacturaProcesoDTO50
                    {
                        Opcion = "ELIMINAR",
                        FacturaID = _FacturaID,
                    };
                    dtFacturas = logica.SP_FacturasProceso50(sendFacturas);
                }

                if (dtFacturas.Rows.Count > 0 && dtFacturas.Rows[0]["Estado"].ToString() == "1")
                {
                    // MessageBox.Show(dtFacturas.Rows[0]["Mensaje"].ToString(), "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MostrarAvisoTemporal("La guia ha sido rechazada exitosamente.");
                }

            }
        }
        private void MostrarAvisoTemporal(string Comentario)
        {
            pAviso.Visible = true;

            timerAviso = new Timer();
            timerAviso.Interval = 5000; // 5 segundos
            timerAviso.Tick += TimerAviso_Tick;
            timerAviso.Start();
            lblComentario.Text = Comentario;
        }
        private void TimerAviso_Tick(object sender, EventArgs e)
        {
            pAviso.Visible = false;
            timerAviso.Stop();
            timerAviso.Dispose();
        }
        private void Validaciones()
        {
            pasaValidacion = true;
            _GeneraError = true;
            _ErrorRecibido = true;
            _CambioError = false;

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
                            break;
                        }
                    }
                }
            }
            //Valido que el total recibido no sea menor al total esperado
            _ErrorRecibido = (Convert.ToDecimal(lblRecibido.Text) < Convert.ToDecimal(lblGranTotal.Text)) ? true : false;

            decimal sumaMetodos = 0;
            decimal sumaEfectivo = 0;
            decimal sumaTotal = 0;

            foreach (DataGridViewRow row in dgvMetodosPago.Rows)
            {
                if (row.IsNewRow) continue; // Evita procesar la fila vacía al final

                int metodoID = Convert.ToInt32(row.Cells["ID"].Value);

                object valorObj = row.Cells["Valor"].Value;
                if (valorObj != null && decimal.TryParse(valorObj.ToString(), out decimal valor))
                {
                    if (metodoID == 3 || metodoID == 5 || metodoID == 6)
                    {
                        sumaMetodos += valor;
                    }
                    else if (metodoID == 4)
                    {
                        sumaEfectivo += valor;
                    }
                }
            }

            // Validación final para saber si puede darle cambio o no
            if (sumaMetodos > Convert.ToDecimal(lblGranTotal.Text))
            {
                if (sumaEfectivo < Convert.ToDecimal(lblCambio.Text))
                {
                    _CambioError = true;
                }
            }
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
                MessageBox.Show("Debe completar el campo de referencia cuando no sea efectivo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_CambioError == true)
            {
                MessageBox.Show("Verifique los montos ingresados porque la suma del total de los metodos no debe ser mayor al total de la factura", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_GeneraError == false) { return; }

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
                Estado = true,
                Origen = dgvFacturas.CurrentRow.Cells["Origen"].Value.ToString(),
            };
            dtFacturasEncabezado = logica.SP_CobroCajaEncabezado(sendEncabezado);
            if (dtFacturasEncabezado.Rows.Count > 0 && dtFacturasEncabezado.Rows[0]["Estado"].ToString() == "1")
            {
                _IDCobroCajaEncabezado = Convert.ToInt32(dtFacturasEncabezado.Rows[0]["UltimoID"]);
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

                    decimal EfectivoRecibido = 0;

                    if (Convert.ToInt32(row.Cells["Id"].Value) == 4 && Convert.ToDecimal(lblCambio.Text) > 0)
                    {
                        monto = Convert.ToDecimal(row.Cells["Valor"]?.Value) - Convert.ToDecimal(lblCambio.Text);
                        EfectivoRecibido = Convert.ToDecimal(row.Cells["Valor"]?.Value);
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
                        Estado = true,
                        EfectivoRecibido = EfectivoRecibido
                    };

                    dtFacturasMetodos = logica.SP_CobroCajaMetodos(sendMetodos);

                    if ((dtFacturasMetodos.Rows.Count > 0 && dtFacturasMetodos.Rows[0]["Estado"].ToString() == "0") || dtFacturasMetodos.Rows.Count <= 0)
                    {
                        MessageBox.Show("Ha ocurrido un error al momento de procesar la solicitud, los cambios se revertiran.", "Notificación", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        //Eliminar el encabezado y cualquier registro coincidente en el metodo
                        CobroCajaEncabezadoDTO deleteEncabezado = new CobroCajaEncabezadoDTO
                        {
                            Opcion = "EliminarEncabezado",
                            ID = _IDCobroCajaEncabezado
                        };
                        dtFacturasEncabezado = logica.SP_CobroCajaEncabezado(deleteEncabezado);
                        if (dtFacturasEncabezado.Rows.Count > 0 && dtFacturas.Rows[0]["Mensaje"].ToString() == "0")
                        {
                            MessageBox.Show(dtFacturas.Rows[0]["Mensaje"].ToString(), "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        //Eliminar cualquier registro que sea ligado al encabezado
                        CobroCajaMetodosDTO deleteMetodos = new CobroCajaMetodosDTO
                        {
                            Opcion = "EliminarPorEncabezado",
                            EncabezadoID = _EncabezadoID,
                        };

                        dtFacturasMetodos = logica.SP_CobroCajaMetodos(deleteMetodos);
                        if (dtFacturasMetodos.Rows.Count > 0 && dtFacturas.Rows[0]["Mensaje"].ToString() == "0")
                        {
                            MessageBox.Show(dtFacturasMetodos.Rows[0]["Mensaje"].ToString(), "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        return;
                    }
                }

                ActualizarEstadoFactura();
                Limpiar();
                CargarMetodoPago();
            }
            else // Hubo clavo desde el encabezado
            {
                MessageBox.Show(dtFacturasMetodos.Rows[0]["Mensaje"].ToString(), "Notificación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void ActualizarTasaFactura2(decimal tasa, string factura)
        {
            Factura2DTO50 setTasa = new Factura2DTO50
            {
                Opcion = "Actualizar",
                FactorDolar = tasa,
                Factura = factura
            };
            dtFacturaUpdateTasa = logica.SP_Facturas250(setTasa);
            if (dtFacturaUpdateTasa.Rows.Count > 0 && dtFacturaUpdateTasa.Rows[0]["Estado"].ToString() == "0")
            {
                MessageBox.Show(dtFacturaUpdateTasa.Rows[0]["Mensaje"].ToString(), "Notificación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ActualizarEstadoFactura()
        {
            if (dgvFacturas.CurrentRow.Cells["Origen"].Value.ToString() == "ENAC.")
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
            } else if (dgvFacturas.CurrentRow.Cells["Origen"].Value.ToString() == "INTER") {

                ActualizarTasaFactura2(DynamicMain.tasa, _FacturaID.ToString());

                int SucursalUbicacion =  ConsultaBodegaExistente();

                if (SucursalUbicacion == 3) 
                {
                    DialogResult resultado = MessageBox.Show("¿Desea enviar la solicitud a bodega para buscar el producto?","Confirmación de envío",MessageBoxButtons.YesNo,MessageBoxIcon.Question);

                    if (resultado == DialogResult.Yes)
                    {
                        // Aquí llamás tu método para enviar la solicitud
                        FacturaProcesoDTO50 sendFacturas50 = new FacturaProcesoDTO50
                        {
                            Opcion = "AGREGAR",
                            FacturaID = _FacturaID,
                            Proceso = 2,
                            UPosteo = DynamicMain.usuarionlogin,
                            FPosteo = DateTime.Now,
                            PC = System.Environment.MachineName,
                            Guia = _GuiaID
                        };
                        dtFacturas = logica.SP_FacturasProceso50(sendFacturas50);

                        if (dtFacturas.Rows.Count > 0 && dtFacturas.Rows[0]["Estado"].ToString() == "1")
                        {
                            MostrarAvisoTemporal("Solicitud enviada a bodega correctamente.");
                            Limpiar();
                        }
                        else
                        {
                            MessageBox.Show("Ha ocurrido un error al procesar la solicitud", "Notificacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else if (resultado == DialogResult.No) 
                    {
                        // Aquí llamás tu método para enviar la solicitud
                        FacturaProcesoDTO50 sendFacturas50 = new FacturaProcesoDTO50
                        {
                            Opcion = "AGREGAR",
                            FacturaID = _FacturaID,
                            Proceso = 2,
                            UPosteo = DynamicMain.usuarionlogin,
                            FPosteo = DateTime.Now,
                            PC = System.Environment.MachineName,
                            Guia = _GuiaID
                        };
                        dtFacturas = logica.SP_FacturasProceso50(sendFacturas50);
                    }
                    if (dtFacturas.Rows.Count > 0 && dtFacturas.Rows[0]["Estado"].ToString() == "1")
                    {
                        MostrarAvisoTemporal("Registro ingresado correctamente.");
                        Limpiar();
                    }
                    else
                    {
                        MessageBox.Show("Ha ocurrido un error al procesar la solicitud", "Notificacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Aquí llamás tu método para enviar la solicitud
                    FacturaProcesoDTO50 sendFacturas50 = new FacturaProcesoDTO50
                    {
                        Opcion = "AGREGAR",
                        FacturaID = _FacturaID,
                        Proceso = 2,
                        UPosteo = DynamicMain.usuarionlogin,
                        FPosteo = DateTime.Now,
                        PC = System.Environment.MachineName,
                        Guia = _GuiaID
                    };
                    dtFacturas = logica.SP_FacturasProceso50(sendFacturas50);
                    if (dtFacturas.Rows.Count > 0 && dtFacturas.Rows[0]["Estado"].ToString() == "1")
                    {
                        MostrarAvisoTemporal("Registro ingresado correctamente.");
                        Limpiar();
                    }
                    else
                    {
                        MessageBox.Show("Ha ocurrido un error al procesar la solicitud", "Notificacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private int ConsultaBodegaExistente() 
        {
            SucursalUbicacionDTO buscar = new SucursalUbicacionDTO
            {
                Opcion = "Recuperar",
                UbicacionID = DynamicMain.usuarioSucursalID
            };
            dtBodegaExistente = logica.SP_SucursalUbicaciones(buscar);
            if (dtBodegaExistente.Rows[0]["CajaActiva"].ToString() == "3") 
            {
                return Convert.ToInt32(dtBodegaExistente.Rows[0]["CajaActiva"].ToString());
            }
            return 0;
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
                lblOrigen.Text = dgvFacturas.CurrentRow.Cells["Origen"].Value.ToString();
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
                // Obtener valores de la fila actual
                var filaActual = dgvMetodosPago.Rows[e.RowIndex];
                var metodoID = filaActual.Cells["Id"].Value;
                var descripcion = filaActual.Cells["Metodo"].Value;
                var icono = filaActual.Cells["Ayuda"].Value;

                // Crear nueva fila con los valores deseados
                int nuevaFilaIndex = e.RowIndex + 1;
                dgvMetodosPago.Rows.Insert(nuevaFilaIndex, metodoID, descripcion, "", "", icono);

                // Opcional: seleccionar la nueva fila o enfocar en "Valor"
                dgvMetodosPago.CurrentCell = dgvMetodosPago.Rows[nuevaFilaIndex].Cells["Valor"];
                dgvMetodosPago.BeginEdit(true);
            }
        }
    }
}
