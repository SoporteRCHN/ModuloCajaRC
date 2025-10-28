using Logica;
using ModuloCajaRC.Reportes;
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

namespace ModuloCajaRC.Facturas
{
    public partial class frmApertura : Form
    {
        DataTable dtPermisos = new DataTable();
        DataTable dtAperturaCaja = new DataTable();
        DataTable dtMetodoPago = new DataTable();
        DataTable dtResumenAperturaCaja = new DataTable();
        DataTable dtResumenValoresEsperados = new DataTable();
        DataTable dtMovimientosCaja = new DataTable();
        DataTable dtResumenMovimientos = new DataTable();
        DataTable dtCierresEntreFechas = new DataTable();
        DataTable dtResumenMovimientosCaja = new DataTable();
        DataTable dtMovimientosPreCierreCaja = new DataTable();
        DataTable dtMovimientosUltimoCierreCaja = new DataTable();
        clsLogica logica = new clsLogica();

        decimal tarjetaApertura, chequeApertura, transferenciaApertura, efectivoApertura, totalApertura, movimientoCheques, movimientoTransferencias, movimientoEfectivo,movimientoTarjetas, movimientoTotal = 0;
        public string textoComentario = "";
        public int UltimoCierre = 0;
        public bool _EstaAperturando = true;
 
        public frmApertura()
        {
            InitializeComponent();
            CargarMetodoPago();
            AperturarCaja();
            verMovimientosEntreFechas(dtpInicio.Value.Date, dtpFinal.Value.Date);
            verCierresEntreFechas(dtpInicio.Value.Date, dtpFinal.Value.Date);
            verResumenApertura();
            verResumenMovimientos();
            verResumenValoresEsperados();
            CargarAcciones(1, pAcciones, 159, 49, 0, TextImageRelation.ImageBeforeText);
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
                                case "Imprimir":
                                    if (UltimoCierre!=0) { GenerarCierre(); }
                                    
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
        private void GenerarCierre()
        {
            Form MensajeAdvertencia = new Form();
            using (frmVerCierre Mensaje = new frmVerCierre(UltimoCierre.ToString(),DynamicMain.usuarioSucursal, System.Environment.MachineName))
            {
                MensajeAdvertencia.StartPosition = FormStartPosition.CenterScreen;
                MensajeAdvertencia.FormBorderStyle = FormBorderStyle.None;
                MensajeAdvertencia.Opacity = .70d;
                MensajeAdvertencia.BackColor = Color.Black;
                MensajeAdvertencia.WindowState = FormWindowState.Maximized;
                MensajeAdvertencia.Location = this.Location;
                MensajeAdvertencia.ShowInTaskbar = false;
                //MensajeAdvertencia.TopMost = true;

                Mensaje.Owner = MensajeAdvertencia;
                MensajeAdvertencia.Show();
                Mensaje.ShowDialog();
                MensajeAdvertencia.Dispose();
            }
        }

        private void CargarMetodoPago()
        {
            dtMetodoPago.Clear();
            dtMetodoPago.Rows.Clear();
            MetodoPagoDTO getMetodo = new MetodoPagoDTO
            {
                Opcion = "LISTADO",
            };
            dtMetodoPago = logica.SP_MetodoPagos(getMetodo);
            if (dtMetodoPago.Rows.Count > 0)
            {
                dgvRegistroValores.Rows.Clear();
                foreach (DataRow row in dtMetodoPago.Rows)
                {
                    dgvRegistroValores.Rows.Add(row["MetodoID"], row["Descripcion"], "0.00", row["Edicion"]);
                }
                foreach (DataGridViewRow row in dgvRegistroValores.Rows)
                {
                    bool esSoloLectura = Convert.ToBoolean(row.Cells["EdicionRegistro"].Value);
                    row.Cells["ValorRegistro"].ReadOnly = esSoloLectura;
                    if (!esSoloLectura) 
                    {
                        row.Cells["ValorRegistro"].Style.ForeColor = Color.FromArgb(162, 44, 36);
                    }
                }
            }
        }
        private void verResumenValoresEsperados()
        {
            ControlCajaDTO sendApertura = new ControlCajaDTO
            {
                Opcion = "ResumenValoresEsperados",
                UPosteo = DynamicMain.usuarionlogin,
                PC = Environment.MachineName,
                Estado = true
            };

            dtResumenValoresEsperados.Clear();
            dtResumenValoresEsperados.Rows.Clear();

            dtResumenValoresEsperados = logica.SP_ControlCaja(sendApertura);
            if (dtResumenValoresEsperados.Rows.Count > 0)
            {
                dgvValoresEsperados.Rows.Clear();

                foreach (DataRow row in dtResumenValoresEsperados.Rows)
                {
                    dgvValoresEsperados.Rows.Add(row["MetodoPagoID"], row["Metodo"], row["Valor"]);
                }

                foreach (DataGridViewRow fila in dgvRegistroValores.Rows)
                {
                    if (fila.Cells["EdicionRegistro"].Value?.ToString().ToLower() == "false")
                    {
                        string metodoID = fila.Cells["MetodoID"].Value?.ToString();

                        foreach (DataGridViewRow esperado in dgvValoresEsperados.Rows)
                        {
                            if (esperado.Cells["PagoID"].Value?.ToString() == metodoID)
                            {
                                fila.Cells["ValorRegistro"].Value = esperado.Cells["ValorEsperado"].Value;
                                break;
                            }
                        }
                    }
                }
                dgvValoresEsperados.Columns["MetodoEsperado"].Width = 150;
                dgvValoresEsperados.Columns["ValorEsperado"].Width = 120;
                dgvValoresEsperados.Columns["Varianza"].Width = 100;
            }
        }
        private void verResumenApertura()
        {
            ControlCajaDTO sendApertura = new ControlCajaDTO
            {
                Opcion = "ResumenAperturaCaja",
                UPosteo = DynamicMain.usuarionlogin,
                PC = System.Environment.MachineName,
                Estado = true
            };

            dtResumenAperturaCaja.Clear();
            dtResumenAperturaCaja.Rows.Clear();

            dtResumenAperturaCaja = logica.SP_ControlCaja(sendApertura);
            if (dtResumenAperturaCaja.Rows.Count > 0)
            {
                foreach (DataRow row in dtResumenAperturaCaja.Rows)
                {
                    dgvResumenApertura.Rows.Add(row["Metodo"], row["Valor"]);
                }
                foreach (DataGridViewRow fila in dgvRegistroValores.Rows)
                {
                    if (fila.Cells["EdicionRegistro"].Value?.ToString().ToLower() == "false")
                    {
                        fila.DefaultCellStyle.ForeColor = Color.FromArgb(162, 44, 36);
                    }
                }
            }
        }
        private void verResumenMovimientos()
        {
            ControlCajaDTO sendApertura = new ControlCajaDTO
            {
                Opcion = "ResumenCajaVertical",
                UPosteo = DynamicMain.usuarionlogin,
                PC = System.Environment.MachineName,
                Estado = true
            };

            dtResumenMovimientosCaja.Clear();
            dtResumenMovimientosCaja.Rows.Clear();

            dtResumenMovimientosCaja = logica.SP_ControlCaja(sendApertura);
            if (dtResumenMovimientosCaja.Rows.Count > 0)
            {
                foreach (DataRow row in dtResumenMovimientosCaja.Rows)
                {
                    dgvResumenMovimientos.Rows.Add(row["Metodo"], row["Valor"]);
                }
            }
        }
        private void AperturarCaja() 
        {
            ControlCajaDTO sendApertura = new ControlCajaDTO
            {
                Opcion = "AperturaCaja",
                UPosteo = DynamicMain.usuarionlogin,
                PC = System.Environment.MachineName
            };

            dtAperturaCaja = logica.SP_ControlCaja(sendApertura);

            if (dtAperturaCaja.Rows.Count > 0) 
            {
                DynamicMain.cajaID = Convert.ToInt32(dtAperturaCaja.Rows[0]["ControlID"].ToString());

                lblFecha.Text = dtAperturaCaja.Rows[0]["FPosteo"].ToString();
                lblEquipo.Text = dtAperturaCaja.Rows[0]["PC"].ToString();
                lblUsuario.Text = dtAperturaCaja.Rows[0]["NombreCompleto"].ToString();

                efectivoApertura = (!String.IsNullOrWhiteSpace(dtAperturaCaja.Rows[0]["MontoEfectivo"].ToString())) ? Convert.ToDecimal(dtAperturaCaja.Rows[0]["MontoEfectivo"]) : 0;

                transferenciaApertura = (!String.IsNullOrWhiteSpace(dtAperturaCaja.Rows[0]["MontoTransferencia"].ToString())) ? Convert.ToDecimal(dtAperturaCaja.Rows[0]["MontoTransferencia"]) : 0;

                chequeApertura = (!String.IsNullOrWhiteSpace(dtAperturaCaja.Rows[0]["MontoCheque"].ToString())) ? Convert.ToDecimal(dtAperturaCaja.Rows[0]["MontoCheque"]): 0;

                tarjetaApertura = (!String.IsNullOrWhiteSpace(dtAperturaCaja.Rows[0]["MontoTarjeta"].ToString())) ? Convert.ToDecimal(dtAperturaCaja.Rows[0]["MontoTarjeta"]) : 0;

                totalApertura = efectivoApertura + transferenciaApertura + chequeApertura + tarjetaApertura;
 
                label9.Text = "Ya existe una apertura de caja realizada.";
                label9.Visible = true;

                lblValoresCierre.Text = "VALORES DE CIERRE";

                btnProceso.BackColor = Color.FromArgb(230, 150, 92);
                btnProceso.Text = "CIERRE";
                
                _EstaAperturando = false;
            }
            else
            {
                lblFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
                lblEquipo.Text = System.Environment.MachineName;
                lblUsuario.Text = DynamicMain.usuarionlogin;

                btnProceso.BackColor = Color.FromArgb(97, 172, 112);
                btnProceso.Text = "APERTURAR";
                _EstaAperturando = true;
            }
        }

        private void EnviarControlCaja(int _TipoID)
        {
            if (_TipoID == 2) 
            {
                Form MensajeAdvertencia = new Form();
                using (frmComentariocCierreCaja Mensaje = new frmComentariocCierreCaja())
                {
                    MensajeAdvertencia.StartPosition = FormStartPosition.CenterScreen;
                    MensajeAdvertencia.FormBorderStyle = FormBorderStyle.None;
                    MensajeAdvertencia.Opacity = .70d;
                    MensajeAdvertencia.BackColor = Color.Black;
                    MensajeAdvertencia.WindowState = FormWindowState.Maximized;
                    MensajeAdvertencia.Location = this.Location;
                    MensajeAdvertencia.ShowInTaskbar = false;
                    //MensajeAdvertencia.TopMost = true;

                    Mensaje.Owner = MensajeAdvertencia;
                    MensajeAdvertencia.Show();
                    Mensaje.ShowDialog();
                    MensajeAdvertencia.Dispose();
                    if (Mensaje.ingresoAlgo == true) 
                    {
                        textoComentario = Mensaje.textoCierre;
                    }
                }
            }
            Dictionary<int, decimal> montosPorMetodo = new Dictionary<int, decimal>();
            Dictionary<int, decimal> varianzaPorMetodo = new Dictionary<int, decimal>();

            foreach (DataGridViewRow rowMontos in dgvRegistroValores.Rows)
            {
                if (rowMontos.IsNewRow) continue;

                if (int.TryParse(rowMontos.Cells["MetodoID"].Value?.ToString(), out int metodoID) &&
                    decimal.TryParse(rowMontos.Cells["ValorRegistro"].Value?.ToString(), out decimal valorMonto))
                {
                    if (montosPorMetodo.ContainsKey(metodoID))
                        montosPorMetodo[metodoID] += valorMonto;
                    else
                        montosPorMetodo[metodoID] = valorMonto;
                }
            }

            foreach (DataGridViewRow rowVarianza in dgvValoresEsperados.Rows)
            {
                if (rowVarianza.IsNewRow) continue;

                if (int.TryParse(rowVarianza.Cells["PagoID"].Value?.ToString(), out int pagoID) &&
                    decimal.TryParse(rowVarianza.Cells["Varianza"].Value?.ToString(), out decimal valorVarianza))
                {
                    if (varianzaPorMetodo.ContainsKey(pagoID))
                        varianzaPorMetodo[pagoID] += valorVarianza;
                    else
                        varianzaPorMetodo[pagoID] = valorVarianza;
                }
            }

            if (_EstaAperturando == true && montosPorMetodo.Values.Sum() == 0)
            {
                return;
            }

            ControlCajaDTO sendApertura = new ControlCajaDTO
            {
                Opcion = "Agregar",
                TipoID = _TipoID,
                MontoCheque = montosPorMetodo.ContainsKey(3) ? montosPorMetodo[3] : 0,
                MontoEfectivo = montosPorMetodo.ContainsKey(4) ? montosPorMetodo[4] : 0,
                MontoTransferencia = montosPorMetodo.ContainsKey(5) ? montosPorMetodo[5] : 0,
                MontoTarjeta = montosPorMetodo.ContainsKey(6) ? montosPorMetodo[6] : 0,
                VarianzaCheque = varianzaPorMetodo.ContainsKey(3) ? varianzaPorMetodo[3] : 0,
                VarianzaEfectivo = varianzaPorMetodo.ContainsKey(4) ? varianzaPorMetodo[4] : 0,
                VarianzaTransferencia = varianzaPorMetodo.ContainsKey(5) ? varianzaPorMetodo[5] : 0,
                VarianzaTarjeta = varianzaPorMetodo.ContainsKey(6) ? varianzaPorMetodo[6] : 0,
                MontoTotal = montosPorMetodo.Values.Sum(),
                UPosteo = DynamicMain.usuarionlogin,
                FPosteo = DateTime.Now,
                PC = Environment.MachineName,
                Estado = true,
                Comentario = textoComentario,
            };


            dtAperturaCaja = logica.SP_ControlCaja(sendApertura);
            if (dtAperturaCaja.Rows.Count > 0 && dtAperturaCaja.Rows[0]["Estado"].ToString() == "1")
            {
                if (_TipoID == 1)
                {
                    DynamicMain.cajaID = Convert.ToInt32(dtAperturaCaja.Rows[0]["UltimoID"].ToString());
                    MessageBox.Show("¡Apertura de caja realizada exitosamente!", "Notificación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AperturarCaja();
                    verResumenApertura();
                    verResumenMovimientos();
                    verResumenValoresEsperados();
                    CargarMetodoPago();
                }
                else if (_TipoID == 2)
                {
                    UltimoCierre = Convert.ToInt32(dtAperturaCaja.Rows[0]["UltimoID"].ToString())-1;
                    GenerarCierre();
                    Limpiar();
                    MessageBox.Show("¡Cierre de caja realizado exitosamente!", "Notificación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void frmApertura_Load(object sender, EventArgs e)
        {
            dgvRegistroValores.Columns["MetodoRegistro"].ReadOnly = true;
            dgvRegistroValores.Columns["ValorRegistro"].ReadOnly = false;
            dgvRegistroValores.EditMode = DataGridViewEditMode.EditOnEnter;
            dgvRegistroValores.CellBeginEdit += dgvRegistroValores_CellBeginEdit;

            for (int i = 0; i < dgvRegistroValores.Rows.Count; i++)
            {
                ActualizarVarianza(i);
            }
            dgvRegistroValores.ClearSelection();
            dgvResumenApertura.ClearSelection();
            dgvResumenMovimientos.ClearSelection();
        }

        private void btnMovimientosActuales_Click(object sender, EventArgs e)
        {
            bool existeApertura = false;
            bool existeCierre = false;
            int idBuscar = Convert.ToInt32(cmbLotes.SelectedValue);
            int idCierre = 0;
            foreach (DataGridViewRow item in dgvMovimientos.Rows)
            {
                if (Convert.ToInt32(item.Cells["ControlID"].Value) == idBuscar) 
                {
                    if (item.Cells["Tipo"].Value.ToString() == "Apertura")
                    { 
                        existeApertura = true;
                        idCierre = idBuscar + 1; 
                    }
                }
                if (idCierre != 0 && Convert.ToInt32(item.Cells["ControlID"].Value) == idCierre) 
                {
                    if (item.Cells["Tipo"].Value.ToString() == "Cierre")
                    {
                        existeCierre = true;
                        
                    }
                }
              }
            if (existeApertura == true && existeCierre == true) 
            {
                Form MensajeAdvertencia = new Form();
                using (frmVerCierre Mensaje = new frmVerCierre((Convert.ToInt32(cmbLotes.SelectedValue)).ToString(), DynamicMain.usuarioSucursal, System.Environment.MachineName))
                {
                    MensajeAdvertencia.StartPosition = FormStartPosition.CenterScreen;
                    MensajeAdvertencia.FormBorderStyle = FormBorderStyle.None;
                    MensajeAdvertencia.Opacity = .70d;
                    MensajeAdvertencia.BackColor = Color.Black;
                    MensajeAdvertencia.WindowState = FormWindowState.Maximized;
                    MensajeAdvertencia.Location = this.Location;
                    MensajeAdvertencia.ShowInTaskbar = false;
                    //MensajeAdvertencia.TopMost = true;

                    Mensaje.Owner = MensajeAdvertencia;
                    MensajeAdvertencia.Show();
                    Mensaje.ShowDialog();
                    MensajeAdvertencia.Dispose();
                }
            }
            else
            {
                MessageBox.Show("Aun no se ha realizado el cierre sobre este lote, no puede imprimirlo.","Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void verMovimientosPreCierre()
        {
            movimientoCheques = 0;
            movimientoEfectivo = 0;
            movimientoTarjetas = 0;
            movimientoTransferencias = 0;
            movimientoTotal = 0;

            ControlCajaDTO sendApertura = new ControlCajaDTO
            {
                Opcion = "ListadoRegistros",
                UPosteo = DynamicMain.usuarionlogin,
                PC = System.Environment.MachineName,
                Estado = true
            };

            dtMovimientosPreCierreCaja.Clear();
            dtMovimientosPreCierreCaja.Rows.Clear();

            dtMovimientosPreCierreCaja = logica.SP_ControlCaja(sendApertura);
            if (dtMovimientosPreCierreCaja.Rows.Count > 0)
            {
                //Cargar Resumen
                foreach (DataRow row in dtMovimientosPreCierreCaja.Rows)
                {
                    if (!String.IsNullOrEmpty(row["Cheque"].ToString())) 
                    {
                        movimientoCheques += Convert.ToDecimal(row["Cheque"]);
                    }

                    if (!String.IsNullOrEmpty(row["Transferencia"].ToString()))
                    {
                        movimientoTransferencias += Convert.ToDecimal(row["Transferencia"]);
                    }

                    if (!String.IsNullOrEmpty(row["Efectivo"].ToString()))
                    {
                        movimientoEfectivo += Convert.ToDecimal(row["Efectivo"]);
                    }

                    if (!String.IsNullOrEmpty(row["Tarjeta"].ToString()))
                    {
                        movimientoTarjetas += Convert.ToDecimal(row["Tarjeta"]);
                    }
                }
               
                movimientoTotal = (movimientoCheques + movimientoTransferencias + movimientoTarjetas + movimientoEfectivo);
            }
        }
        private void CierreEspecifico()
        {
            ControlCajaDTO sendApertura = new ControlCajaDTO
            {
                Opcion = "MostrarCierreEspecifico",
                ControlID = Convert.ToInt64(cmbLotes.SelectedValue.ToString())
            };

            dtMovimientosCaja.Clear();
            dtMovimientosCaja.Rows.Clear();

            dtMovimientosCaja = logica.SP_ControlCaja(sendApertura);
            if (dtMovimientosCaja.Rows.Count > 0)
            {
                dgvMovimientos.DataSource = dtMovimientosCaja;
                //dgvMovimientos.Columns["LoteNro"].Visible = false;
                dgvMovimientos.Columns["Tipo"].Width = 150;
                dgvMovimientos.Columns["Fecha"].Width = 150;
                dgvMovimientos.Columns["FacturaID"].Width = 150;
                dgvMovimientos.Columns["Cheque"].Width = 150;
                dgvMovimientos.Columns["Efectivo"].Width = 150;
                dgvMovimientos.Columns["Transferencia"].Width = 150;
                dgvMovimientos.Columns["Tarjeta"].Width = 150;

                //Cargar Resumen Movimientos
                CargarResumenMovimientos();
            }
        }
        private void ListadoLoteActual() 
        {
            ControlCajaDTO sendApertura = new ControlCajaDTO
            {
                Opcion = "ListadoLoteActual",
                FechaInicio = dtpInicio.Value.Date,
                FechaFinal = dtpFinal.Value.Date,
                UPosteo = DynamicMain.usuarionlogin,
                PC = System.Environment.MachineName
            };

            dtMovimientosCaja.Clear();
            dtMovimientosCaja.Rows.Clear();

            dtMovimientosCaja = logica.SP_ControlCaja(sendApertura);
            if (dtMovimientosCaja.Rows.Count > 0)
            {
                dgvMovimientos.DataSource = dtMovimientosCaja;
                //dgvMovimientos.Columns["LoteNro"].Visible = false;
                dgvMovimientos.Columns["Tipo"].Width = 150;
                dgvMovimientos.Columns["Fecha"].Width = 150;
                dgvMovimientos.Columns["FacturaID"].Width = 150;
                dgvMovimientos.Columns["Cheque"].Width = 150;
                dgvMovimientos.Columns["Efectivo"].Width = 150;
                dgvMovimientos.Columns["Transferencia"].Width = 150;
                dgvMovimientos.Columns["Tarjeta"].Width = 150;


                //Cargar Resumen Movimientos
                CargarResumenMovimientos();
            }
        }
        private void dgvRegistroValores_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRegistroValores.Columns[e.ColumnIndex].Name == "ValorRegistro")
            {
                var celda = dgvRegistroValores.Rows[e.RowIndex].Cells["ValorRegistro"];
                var valor = celda.Value?.ToString();

                if (string.IsNullOrWhiteSpace(valor))
                {
                    celda.Value = "0.00";
                }
            }
        }
        private void dgvRegistroValores_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRegistroValores.Columns[e.ColumnIndex].Name == "ValorRegistro")
            {
               // MessageBox.Show(DynamicMain.cajaID.ToString());
                ActualizarVarianza(e.RowIndex);
            }
        }
        private void ActualizarVarianza(int filaIndex)
        {
            if (_EstaAperturando) return;

            var filaRegistro = dgvRegistroValores.Rows[filaIndex];
            string metodoID = filaRegistro.Cells["MetodoID"].Value?.ToString();
            string valorTexto = filaRegistro.Cells["ValorRegistro"].Value?.ToString();

            if (!decimal.TryParse(valorTexto, out decimal valorRegistro))
            {
                valorRegistro = 0;
            }

            foreach (DataGridViewRow filaEsperado in dgvValoresEsperados.Rows)
            {
                string pagoID = filaEsperado.Cells["PagoID"].Value?.ToString();
                if (pagoID == metodoID)
                {
                    string esperadoTexto = filaEsperado.Cells["ValorEsperado"].Value?.ToString();
                    if (!decimal.TryParse(esperadoTexto, out decimal valorEsperado))
                    {
                        valorEsperado = 0;
                    }

                    decimal varianza = valorRegistro - valorEsperado;
                    filaEsperado.Cells["Varianza"].Value = varianza.ToString("N2");

                    if (Convert.ToDouble(filaEsperado.Cells["Varianza"].Value) < 0)
                    {
                        filaEsperado.DefaultCellStyle.ForeColor = Color.FromArgb(162, 44, 36);
                    }
                    else
                if (Convert.ToDouble(filaEsperado.Cells["Varianza"].Value) >= 0)
                    {
                        filaEsperado.DefaultCellStyle.ForeColor = Color.FromArgb(0,0,0);
                    }
                    break;
                }
            }
            dgvValoresEsperados.ClearSelection();
        }

        private void btnMovimientos_Click(object sender, EventArgs e)
        {
            ListadoLoteActual();
        }

        private void CargarResumenMovimientos() 
        {
            movimientoCheques = 0;
            movimientoEfectivo = 0;
            movimientoTarjetas = 0;
            movimientoTransferencias = 0;

            //Cargar Resumen
            foreach (DataGridViewRow row in dgvMovimientos.Rows)
            {
                movimientoCheques += row.Cells["Cheque"].Value != DBNull.Value ? Convert.ToDecimal(row.Cells["Cheque"].Value) : 0;
                movimientoTransferencias += row.Cells["Transferencia"].Value != DBNull.Value ? Convert.ToDecimal(row.Cells["Transferencia"].Value) : 0;
                movimientoEfectivo += row.Cells["Efectivo"].Value != DBNull.Value ? Convert.ToDecimal(row.Cells["Efectivo"].Value) : 0;
                movimientoTarjetas += row.Cells["Tarjeta"].Value != DBNull.Value ? Convert.ToDecimal(row.Cells["Tarjeta"].Value) : 0;
            }

            lblCheque.Text = movimientoCheques.ToString("N2");
            lblTransferencia.Text = movimientoTransferencias.ToString("N2");
            lblTarjeta.Text = movimientoTarjetas.ToString("N2");
            lblEfectivo.Text = movimientoEfectivo.ToString("N2");

            lblRegistros.Text = dgvMovimientos.Rows.Count.ToString();
            lblTotalIngresos.Text = (movimientoCheques + movimientoTransferencias + movimientoEfectivo + movimientoTarjetas).ToString("N2");
        }

        private void dgvMovimientos_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvMovimientos.Rows)
            {
                string tipo = row.Cells["Tipo"].Value?.ToString()?.Trim();

                if (tipo == "Apertura")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(143, 171, 190); // Verde
                    row.DefaultCellStyle.ForeColor = Color.White;
                }
                else if (tipo == "Cierre")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(54, 84, 134); // Rojo
                    row.DefaultCellStyle.ForeColor = Color.White;
                }
            }
            dgvMovimientos.ClearSelection();
        }

        private void cmbLotes_SelectionChangeCommitted(object sender, EventArgs e)
        {
            MessageBox.Show(cmbLotes.SelectedValue.ToString());
            CierreEspecifico();
        }

        private void dtpFinal_ValueChanged(object sender, EventArgs e)
        {
            if (dtpFinal.Value.Date < dtpInicio.Value.Date)
            {
                dtpFinal.Value = dtpInicio.Value;
            }

            verMovimientosEntreFechas(dtpInicio.Value.Date, dtpFinal.Value.Date);
            verCierresEntreFechas(dtpInicio.Value.Date, dtpFinal.Value.Date);
        }
        private void dtpInicio_ValueChanged(object sender, EventArgs e)
        {
            if (dtpFinal.Value.Date < dtpInicio.Value.Date)
            {
                dtpFinal.Value = dtpInicio.Value;
            }
            verMovimientosEntreFechas(dtpInicio.Value.Date, dtpFinal.Value.Date);
            verCierresEntreFechas(dtpInicio.Value.Date, dtpFinal.Value.Date);
        }
        private void verCierresEntreFechas(DateTime Inicio, DateTime Final)
        {
            ControlCajaDTO sendApertura = new ControlCajaDTO
            {
                Opcion = "CierresEntreFechas",
                FechaInicio = Inicio,
                FechaFinal = Final,
            };

            dtCierresEntreFechas.Clear();
            dtCierresEntreFechas.Rows.Clear();

            dtCierresEntreFechas = logica.SP_ControlCaja(sendApertura);
            if (dtCierresEntreFechas.Rows.Count > 0)
            {
                cmbLotes.DataSource = dtCierresEntreFechas;
                cmbLotes.DisplayMember = "Descripcion";
                cmbLotes.ValueMember = "ControlID";
            }
        }
        private void verMovimientosEntreFechas(DateTime Inicio, DateTime Final)
        {
            ControlCajaDTO sendApertura = new ControlCajaDTO
            {
                Opcion = "ListadoRegistrosFechas",
                FechaInicio = Inicio,
                FechaFinal = Final,
                UPosteo = DynamicMain.usuarionlogin,
                PC = System.Environment.MachineName
            };

            dtMovimientosCaja.Clear();
            dtMovimientosCaja.Rows.Clear();

            dtMovimientosCaja = logica.SP_ControlCaja(sendApertura);

            if (dtMovimientosCaja.Rows.Count > 0)
            {
                dgvMovimientos.DataSource = dtMovimientosCaja;

                // Configurar columnas
                dgvMovimientos.Columns["LoteNro"].Visible = true;
                dgvMovimientos.Columns["EncabezadoID"].Visible = true;
                dgvMovimientos.Columns["ControlID"].Visible = true;
                dgvMovimientos.Columns["Tipo"].Width = 150;
                dgvMovimientos.Columns["Fecha"].Width = 150;
                dgvMovimientos.Columns["FacturaID"].Width = 150;
                dgvMovimientos.Columns["Cheque"].Width = 150;
                dgvMovimientos.Columns["Efectivo"].Width = 150;
                dgvMovimientos.Columns["Transferencia"].Width = 150;
                dgvMovimientos.Columns["Tarjeta"].Width = 150;

             
                // Cargar resumen
                CargarResumenMovimientos();
                
            }
            else
            {
                dgvMovimientos.DataSource = null;

                lblCheque.Text = "0.00";
                lblTransferencia.Text = "0.00";
                lblTarjeta.Text = "0.00";
                lblEfectivo.Text = "0.00";
                lblRegistros.Text = "0";
                lblTotalIngresos.Text = "0.00";
            }
        }

        private void dgvRegistroValores_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvRegistroValores.CurrentCell.ColumnIndex == dgvRegistroValores.Columns["ValorRegistro"].Index)
            {
                TextBox txt = e.Control as TextBox;
                if (txt != null)
                {
                    txt.KeyPress -= SoloNumeros;
                    txt.KeyPress += SoloNumeros;

                    // 🔥 Limpieza si el valor es 0.00
                    string valorActual = dgvRegistroValores.CurrentCell.Value?.ToString();
                    if (valorActual == "0.00" || string.IsNullOrWhiteSpace(valorActual))
                    {
                        txt.Text = "";
                        txt.Select(); // activa el cursor
                    }
                    else
                    {
                        txt.SelectAll(); // selecciona todo para editar fácilmente
                    }
                }
            }
        }

        private void SoloNumeros(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            TextBox txt = sender as TextBox;
            if (e.KeyChar == '.' && txt.Text.Contains("."))
            {
                e.Handled = true;
            }
        }
        private void dgvRegistroValores_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dgvRegistroValores.Columns[e.ColumnIndex].Name == "ValorRegistro")
            {
                var edicion = dgvRegistroValores.Rows[e.RowIndex].Cells["EdicionRegistro"].Value?.ToString();

                if (edicion != null && edicion.ToLower() == "false")
                {
                    e.Cancel = true;
                }
            }
        }

        private void btnProceso_Click(object sender, EventArgs e)
        {
            if (_EstaAperturando == true) 
            { EnviarControlCaja(1); } 
            else 
            {
                EnviarControlCaja(2); 
            }
        }

        private void Limpiar() 
        {
            lblFecha.Text = "-";
            lblEquipo.Text = "-";
            lblUsuario.Text = "-";

            label9.Visible = false;

            btnProceso.BackColor = Color.FromArgb(97, 172, 112);
            btnProceso.Text = "APERTURAR";

            //Regreso la variable para apertura
            _EstaAperturando = true;

            CargarMetodoPago();
            dgvValoresEsperados.Rows.Clear();
            dgvResumenApertura.Rows.Clear();
            dgvResumenMovimientos.Rows.Clear();
        }
    }
}
