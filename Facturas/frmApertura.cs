using Logica;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModuloCajaRC.Facturas
{
    public partial class frmApertura : Form
    {
        DataTable dtAperturaCaja = new DataTable();
        DataTable dtMetodoPago = new DataTable();
        DataTable dtResumenAperturaCaja = new DataTable();
        DataTable dtResumenValoresEsperados = new DataTable();
        DataTable dtMovimientosCaja = new DataTable();
        DataTable dtResumenMovimientosCaja = new DataTable();
        DataTable dtMovimientosPreCierreCaja = new DataTable();
        DataTable dtMovimientosUltimoCierreCaja = new DataTable();
        clsLogica logica = new clsLogica();

        decimal tarjetaApertura, chequeApertura, transferenciaApertura, efectivoApertura, totalApertura, movimientoCheques, movimientoTransferencias, movimientoEfectivo,movimientoTarjetas, movimientoTotal = 0;

        public bool _EstaAperturando = true;
        public frmApertura()
        {
            InitializeComponent();
            CargarMetodoPago();
            AperturarCaja();
            verMovimientos();
            verResumenApertura();
            verResumenMovimientos();
            verResumenValoresEsperados();
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
                    dgvRegistroValores.Rows.Add(row["MetodoID"], row["Descripcion"], "0.00", row["Edicion"]);
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
                        fila.DefaultCellStyle.BackColor = Color.FromArgb(198, 204, 209);
                        fila.Cells["ValorRegistro"].Style.ForeColor = Color.Black;
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
            Dictionary<int, decimal> montosPorMetodo = new Dictionary<int, decimal>();

            foreach (DataGridViewRow row in dgvRegistroValores.Rows)
            {
                if (row.IsNewRow) continue;

                if (int.TryParse(row.Cells["MetodoID"].Value?.ToString(), out int metodoID) &&
                    decimal.TryParse(row.Cells["ValorRegistro"].Value?.ToString(), out decimal valor))
                {
                    if (montosPorMetodo.ContainsKey(metodoID))
                        montosPorMetodo[metodoID] += valor;
                    else
                        montosPorMetodo[metodoID] = valor;
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
                MontoTotal = montosPorMetodo.Values.Sum(),
                UPosteo = DynamicMain.usuarionlogin,
                FPosteo = DateTime.Now,
                PC = Environment.MachineName,
                Estado = true
            };


            dtAperturaCaja = logica.SP_ControlCaja(sendApertura);
            if (dtAperturaCaja.Rows.Count > 0 && dtAperturaCaja.Rows[0]["Estado"].ToString() == "1")
            {
                if (_TipoID == 1)
                {
                    DynamicMain.cajaID = Convert.ToInt32(dtAperturaCaja.Rows[0]["UltimoID"].ToString());
                    MessageBox.Show("¡Apertura de caja realizada exitosamente!", "Notificación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (_TipoID == 2)
                {
                    MessageBox.Show("¡Cierre de caja realizado exitosamente!", "Notificación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                AperturarCaja();
                verMovimientos();
                Limpiar();
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMovimientosActuales_Click(object sender, EventArgs e)
        {
            verMovimientosPreCierre();
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
        private void verMovimientos() 
        {
            ControlCajaDTO sendApertura = new ControlCajaDTO
            {
                Opcion = "ListadoRegistros",
                UPosteo = DynamicMain.usuarionlogin,
                PC = System.Environment.MachineName,
                Estado = true
            };

            dtMovimientosCaja.Clear();
            dtMovimientosCaja.Rows.Clear();

            dtMovimientosCaja = logica.SP_ControlCaja(sendApertura);
            if (dtMovimientosCaja.Rows.Count > 0) 
            {
                dgvMovimientos.DataSource = dtMovimientosCaja;
                dgvMovimientos.Columns["ControlID"].Visible = false;
                dgvMovimientos.Columns["EncabezadoID"].Visible = false;
                dgvMovimientos.Columns["Factura"].Width = 250;
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
                    break;
                }
            }
        }


        private void CargarResumenMovimientos() 
        {
            //Cargar Resumen
            foreach (DataRow row in dtMovimientosCaja.Rows)
            {
                movimientoCheques += row["Cheque"] != DBNull.Value ? Convert.ToDecimal(row["Cheque"]) : 0;
                movimientoTransferencias += row["Transferencia"] != DBNull.Value ? Convert.ToDecimal(row["Transferencia"]) : 0;
                movimientoEfectivo += row["Efectivo"] != DBNull.Value ? Convert.ToDecimal(row["Efectivo"]) : 0;
                movimientoTarjetas += row["Tarjeta"] != DBNull.Value ? Convert.ToDecimal(row["Tarjeta"]) : 0;
            }

            lblCheque.Text = movimientoCheques.ToString("N2");
            lblTransferencia.Text = movimientoTransferencias.ToString("N2");
            lblTarjeta.Text = movimientoTarjetas.ToString("N2");
            lblEfectivo.Text = movimientoEfectivo.ToString("N2");

            lblRegistros.Text = dtMovimientosCaja.Rows.Count.ToString();
            lblTotalIngresos.Text = (movimientoCheques + movimientoTransferencias + movimientoEfectivo + movimientoTarjetas).ToString("N2");
        }
       
        private void dtpFinal_ValueChanged(object sender, EventArgs e)
        {
            if (dtpFinal.Value < dtpInicio.Value)
            {
                MessageBox.Show("La fecha final no puede ser menor que la fecha de inicio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpFinal.Value = dtpInicio.Value;
            }

            verMovimientosEntreFechas(dtpInicio.Value.Date, dtpFinal.Value.Date);
        }
        private void dtpInicio_ValueChanged(object sender, EventArgs e)
        {
            dtpFinal.MinDate = dtpInicio.Value;
            verMovimientosEntreFechas(dtpInicio.Value.Date, dtpFinal.Value.Date);
        }
        private void verMovimientosEntreFechas(DateTime Inicio, DateTime Final)
        {
            ControlCajaDTO sendApertura = new ControlCajaDTO
            {
                Opcion = "ListadoRegistrosFechas",
                FechaInicio = Inicio,
                FechaFinal = Final,
            };

            dtMovimientosCaja.Clear();
            dtMovimientosCaja.Rows.Clear();

            dtMovimientosCaja = logica.SP_ControlCaja(sendApertura);
            if (dtMovimientosCaja.Rows.Count > 0)
            {
                dgvMovimientos.DataSource = dtMovimientosCaja;
                dgvMovimientos.Columns["ControlID"].Visible = false;
                dgvMovimientos.Columns["EncabezadoID"].Visible = false;
                dgvMovimientos.Columns["Factura"].Width = 250;
                dgvMovimientos.Columns["Cheque"].Width = 150;
                dgvMovimientos.Columns["Efectivo"].Width = 150;
                dgvMovimientos.Columns["Transferencia"].Width = 150;
                dgvMovimientos.Columns["Tarjeta"].Width = 150;

                //Cargar Resumen Movimientos
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

            button1.Enabled = false;
            label9.Visible = false;

            btnProceso.BackColor = Color.FromArgb(97, 172, 112);
            btnProceso.Text = "APERTURAR";

            //Regreso la variable para apertura
            _EstaAperturando = true;
        }
    }
}
