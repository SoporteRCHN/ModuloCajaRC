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
        DataTable dtMovimientosCaja = new DataTable();
        DataTable dtMovimientosPreCierreCaja = new DataTable();
        DataTable dtMovimientosUltimoCierreCaja = new DataTable();
        clsLogica logica = new clsLogica();

        decimal tarjetaApertura, chequeApertura, transferenciaApertura, efectivoApertura, totalApertura, movimientoCheques, movimientoTransferencias, movimientoEfectivo,movimientoTarjetas, movimientoTotal = 0;

        public bool _EstaAperturando = true;
        public frmApertura()
        {
            InitializeComponent();
            AperturarCaja();
            verMovimientos();
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
                lblFecha.Text = dtAperturaCaja.Rows[0]["FPosteo"].ToString();
                lblEquipo.Text = dtAperturaCaja.Rows[0]["PC"].ToString();
                lblUsuario.Text = dtAperturaCaja.Rows[0]["NombreCompleto"].ToString();

                efectivoApertura = (!String.IsNullOrWhiteSpace(dtAperturaCaja.Rows[0]["MontoEfectivo"].ToString())) ? Convert.ToDecimal(dtAperturaCaja.Rows[0]["MontoEfectivo"]) : 0;

                transferenciaApertura = (!String.IsNullOrWhiteSpace(dtAperturaCaja.Rows[0]["MontoTransferencia"].ToString())) ? Convert.ToDecimal(dtAperturaCaja.Rows[0]["MontoTransferencia"]) : 0;

                chequeApertura = (!String.IsNullOrWhiteSpace(dtAperturaCaja.Rows[0]["MontoCheque"].ToString())) ? Convert.ToDecimal(dtAperturaCaja.Rows[0]["MontoCheque"]): 0;

                tarjetaApertura = (!String.IsNullOrWhiteSpace(dtAperturaCaja.Rows[0]["MontoTarjeta"].ToString())) ? Convert.ToDecimal(dtAperturaCaja.Rows[0]["MontoTarjeta"]) : 0;

                totalApertura = efectivoApertura + transferenciaApertura + chequeApertura + tarjetaApertura;

                lblEfectivoInicial.Text = efectivoApertura.ToString("N2");
                lblChequeInicial.Text = chequeApertura.ToString("N2");
                lblTransferenciaInicial.Text = transferenciaApertura.ToString("N2");
                lblTarjetaInicial.Text = tarjetaApertura.ToString("N2");
                lblTotalInicial.Text = totalApertura.ToString("N2");

                txtMonto.ReadOnly = false;
                txtCheque.ReadOnly = false;
                txtTarjeta.ReadOnly = false;
                txtTransferencia.ReadOnly = false;
                txtTotal.ReadOnly = true;

                label9.Text = "Ya existe una apertura de caja realizada.";
                label9.Visible = true;

                lblValoresCierre.Text = "VALORES DE CIERRE";

                txtCheque.Text = "0.00";
                txtTransferencia.Text = "0.00";
                txtTarjeta.Text = "0.00";
                txtMonto.Text = "0.00";
                txtTotal.Text = "0.00";

                btnProceso.BackColor = Color.FromArgb(230, 150, 92);
                btnProceso.Text = "CIERRE";
                
                _EstaAperturando = false;
            }
            else
            {
                lblFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
                lblEquipo.Text = System.Environment.MachineName;
                lblUsuario.Text = DynamicMain.usuarionlogin;

                txtCheque.Text = "0.00";
                txtTarjeta.Text = "0.00";
                txtTransferencia.Text = "0.00";
                txtMonto.Text = "0.00";
                txtTotal.Text = "0.00";

                btnProceso.BackColor = Color.FromArgb(97, 172, 112);
                btnProceso.Text = "APERTURAR";
                _EstaAperturando = true;
            }
        }
        private void EnviarControlCaja(int _TipoID)
        {
            if(_EstaAperturando == true) 
            {
                if (txtCheque.Text == "0.00" && txtMonto.Text == "0.00" && txtTarjeta.Text == "0.00" && txtTransferencia.Text == "0.00" && txtTotal.Text == "0.00") 
                {
                    return;
                }
            }
            ControlCajaDTO sendApertura = new ControlCajaDTO
            {
                Opcion = "Agregar",
                TipoID = _TipoID, //ID para Apertura o Cierre
                MontoCheque = (!String.IsNullOrWhiteSpace(txtCheque.Text)) ? Convert.ToDecimal(txtCheque.Text) : 0,
                MontoEfectivo = (!String.IsNullOrWhiteSpace(txtMonto.Text)) ? Convert.ToDecimal(txtMonto.Text) : 0,
                MontoTarjeta = (!String.IsNullOrWhiteSpace(txtTarjeta.Text)) ? Convert.ToDecimal(txtTarjeta.Text) : 0,
                MontoTransferencia = (!String.IsNullOrWhiteSpace(txtTransferencia.Text)) ? Convert.ToDecimal(txtTransferencia.Text) : 0,
                MontoTotal = (!String.IsNullOrWhiteSpace(txtTotal.Text)) ? Convert.ToDecimal(txtTotal.Text) : 0,
                UPosteo = DynamicMain.usuarionlogin,
                FPosteo = DateTime.Now,
                PC = System.Environment.MachineName,
                Estado = true
            };
            dtAperturaCaja = logica.SP_ControlCaja(sendApertura);
            if (dtAperturaCaja.Rows.Count > 0 && dtAperturaCaja.Rows[0]["Estado"].ToString() == "1")
            {
                if(_TipoID == 1) 
                {
                    DynamicMain.cajaID = Convert.ToInt32(dtAperturaCaja.Rows[0]["UltimoID"].ToString());
                    MessageBox.Show("Apertura de caja realizado exitosamente!", "Notificacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (_TipoID == 2)
                {
                    MessageBox.Show("Cierre de caja realizado exitosamente!", "Notificacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                AperturarCaja();
                verMovimientos();

                Limpiar();
            }
        }
  

        private void frmApertura_Load(object sender, EventArgs e)
        {

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
                
                preCierreCheque.Text = movimientoCheques.ToString("N2");
                preCierreTransferencia.Text = movimientoTransferencias.ToString("N2");
                preCierreTarjeta.Text = movimientoTarjetas.ToString("N2");
                preCierreEfectivo.Text = movimientoEfectivo.ToString("N2");

                movimientoTotal = (movimientoCheques + movimientoTransferencias + movimientoTarjetas + movimientoEfectivo);

                preCierreTotal.Text = movimientoTotal.ToString("N2");
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

                lblSaldoCaja.Text = (Convert.ToDecimal(lblEfectivoInicial.Text) + movimientoEfectivo).ToString("N2");
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

        private void txtTransferencia_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;

            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || e.KeyChar == '.' && !txt.Text.Contains("."))
            {
                return;
            }

            e.Handled = true;
        }

        private void txtTarjeta_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;

            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || e.KeyChar == '.' && !txt.Text.Contains("."))
            {
                return;
            }

            e.Handled = true;
        }

        private void txtCheque_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;

            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || e.KeyChar == '.' && !txt.Text.Contains("."))
            {
                return;
            }

            e.Handled = true;
        }

        private void txtMonto_Leave(object sender, EventArgs e)
        {
            txtMonto.Text = (!String.IsNullOrWhiteSpace(txtMonto.Text)) ? txtMonto.Text : "0.00";
        }

        private void txtTransferencia_Leave(object sender, EventArgs e)
        {
            txtTransferencia.Text = (!String.IsNullOrWhiteSpace(txtTransferencia.Text)) ? txtTransferencia.Text : "0.00";
        }

        private void txtTarjeta_Leave(object sender, EventArgs e)
        {
            txtTarjeta.Text = (!String.IsNullOrWhiteSpace(txtTarjeta.Text)) ? txtTarjeta.Text : "0.00";
        }

        private void txtCheque_Leave(object sender, EventArgs e)
        {
            txtCheque.Text = (!String.IsNullOrWhiteSpace(txtCheque.Text)) ? txtCheque.Text : "0.00";
        }

        private void txtMonto_Enter(object sender, EventArgs e)
        {
            txtMonto.Text = (!String.IsNullOrWhiteSpace(txtMonto.Text) && Convert.ToDecimal(txtMonto.Text) != 0) ? txtMonto.Text : String.Empty;
            if (pbxEfectivo.Visible == true) { pbxEfectivo.Visible = false; }
        }

        private void txtTransferencia_Enter(object sender, EventArgs e)
        {
            txtTransferencia.Text = (!String.IsNullOrWhiteSpace(txtTransferencia.Text) && Convert.ToDecimal(txtTransferencia.Text) != 0) ? txtTransferencia.Text : String.Empty;
            if (pbxTransferencia.Visible == true) { pbxTransferencia.Visible = false; }
        }

        private void txtTarjeta_Enter(object sender, EventArgs e)
        {
            txtTarjeta.Text = (!String.IsNullOrWhiteSpace(txtTarjeta.Text) && Convert.ToDecimal(txtTarjeta.Text) != 0) ? txtTarjeta.Text : String.Empty;
            if (pbxTarjeta.Visible == true) { pbxTarjeta.Visible = false; }
        }

        private void txtCheque_Enter(object sender, EventArgs e)
        {
            txtCheque.Text = (!String.IsNullOrWhiteSpace(txtCheque.Text) && Convert.ToDecimal(txtCheque.Text)!=0) ? txtCheque.Text : String.Empty;
            if (pbxCheque.Visible == true) { pbxCheque.Visible = false; }
        }

        private void btnUltimoCierre_Click(object sender, EventArgs e)
        {
            verMovimientosUltimoCierre();
        }
        private void verMovimientosUltimoCierre()
        {
            movimientoCheques = 0;
            movimientoEfectivo = 0;
            movimientoTarjetas = 0;
            movimientoTransferencias = 0;
            movimientoTotal = 0;

            ControlCajaDTO sendApertura = new ControlCajaDTO
            {
                Opcion = "ListadoUltimoCierre",
                ControlID = DynamicMain.cajaID
            };

            dtMovimientosUltimoCierreCaja.Clear();
            dtMovimientosUltimoCierreCaja.Rows.Clear();

            dtMovimientosUltimoCierreCaja = logica.SP_ControlCaja(sendApertura);
            if (dtMovimientosUltimoCierreCaja.Rows.Count > 0)
            {


                //Cargar Resumen
                foreach (DataRow row in dtMovimientosUltimoCierreCaja.Rows)
                {
                    if (!String.IsNullOrEmpty(row["MontoCheque"].ToString()))
                    {
                        movimientoCheques += Convert.ToDecimal(row["MontoCheque"]);
                    }

                    if (!String.IsNullOrEmpty(row["MontoTransferencia"].ToString()))
                    {
                        movimientoTransferencias += Convert.ToDecimal(row["MontoTransferencia"]);
                    }

                    if (!String.IsNullOrEmpty(row["MontoEfectivo"].ToString()))
                    {
                        movimientoEfectivo += Convert.ToDecimal(row["MontoEfectivo"]);
                    }

                    if (!String.IsNullOrEmpty(row["MontoTarjeta"].ToString()))
                    {
                        movimientoTarjetas += Convert.ToDecimal(row["MontoTarjeta"]);
                    }
                }

                txtCheque.Text = movimientoCheques.ToString("N2");

                //
                txtTransferencia.Text = movimientoTransferencias.ToString("N2");
                txtTarjeta.Text = movimientoTarjetas.ToString("N2");
                txtMonto.Text = movimientoEfectivo.ToString("N2");

                movimientoTotal = (movimientoCheques + movimientoTransferencias + movimientoTarjetas + movimientoEfectivo);

                txtTotal.Text = movimientoTotal.ToString("N2");
                txtMonto.ReadOnly = true;
                
                btnProceso.Enabled = false;
            }
        }
        private void btnArqueo_Click(object sender, EventArgs e)
        {
            verMovimientosPreCierre();

            preCierreCheque.Visible = true;
            preCierreEfectivo.Visible = true;
            preCierreTarjeta.Visible = true;
            preCierreTransferencia.Visible = true;
            preCierreTotal.Visible = true;

            pbxCheque.Visible=false;
            pbxEfectivo.Visible=false;
            pbxTarjeta.Visible=false;
            pbxTransferencia.Visible=false;
        }

        private void btnProceso_Click(object sender, EventArgs e)
        {
            if (_EstaAperturando == true) 
            { EnviarControlCaja(1); } 
            else 
            {
                verMovimientosPreCierre();

                pbxEfectivo.Visible = (movimientoEfectivo != Convert.ToDecimal(txtMonto.Text)) ? true : false;
                pbxCheque.Visible = (movimientoCheques != Convert.ToDecimal(txtCheque.Text)) ? true : false;
                pbxTransferencia.Visible = (movimientoTransferencias != Convert.ToDecimal(txtTransferencia.Text)) ? true : false;
                pbxTarjeta.Visible = (movimientoTarjetas != Convert.ToDecimal(txtTarjeta.Text)) ? true : false;

                if (Convert.ToDecimal(txtMonto.Text) >= movimientoEfectivo) { preCierreEfectivo.Visible = false; } else{ preCierreEfectivo.Visible = true; }
                if (Convert.ToDecimal(txtCheque.Text) >= movimientoCheques) { preCierreCheque.Visible = false; }else { preCierreCheque.Visible = true; }
                if (Convert.ToDecimal(txtTransferencia.Text) >= movimientoTransferencias) { preCierreTransferencia.Visible = false; }else { preCierreTransferencia.Visible = true; }
                if (Convert.ToDecimal(txtTarjeta.Text) >= movimientoTarjetas) { preCierreTarjeta.Visible = false; } else { preCierreTarjeta.Visible = true; }

                if (Convert.ToDecimal(txtMonto.Text) < movimientoEfectivo || Convert.ToDecimal(txtCheque.Text) < movimientoCheques || Convert.ToDecimal(txtTransferencia.Text) < movimientoTransferencias || Convert.ToDecimal(txtTarjeta.Text) < movimientoTarjetas) 
                {
                    MessageBox.Show("Hay valores que no cuadran con lo registrado en sistema, Favor verifique","Aviso",MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                EnviarControlCaja(2); 
            }
        }
        private void RecalculoTotales() 
        {
            efectivoApertura = (!String.IsNullOrWhiteSpace(txtMonto.Text)) ? Convert.ToDecimal(txtMonto.Text) : 0;
            transferenciaApertura = (!String.IsNullOrWhiteSpace(txtTransferencia.Text)) ? Convert.ToDecimal(txtTransferencia.Text) : 0;
            chequeApertura = (!String.IsNullOrWhiteSpace(txtCheque.Text)) ? Convert.ToDecimal(txtCheque.Text) : 0;
            tarjetaApertura = (!String.IsNullOrWhiteSpace(txtTarjeta.Text)) ? Convert.ToDecimal(txtTarjeta.Text) : 0;

            totalApertura = efectivoApertura+ transferenciaApertura+chequeApertura+tarjetaApertura;

        }
        private void txtMonto_KeyUp(object sender, KeyEventArgs e)
        {
            if (_EstaAperturando == true) 
            {
                RecalculoTotales();
                txtTotal.Text = (totalApertura).ToString("N2");
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(txtMonto.Text) && (Convert.ToDecimal(preCierreEfectivo.Text) <= Convert.ToDecimal(txtMonto.Text)))
                {
                    preCierreEfectivo.Visible = false;
                }
            }
        }

        private void txtTransferencia_KeyUp(object sender, KeyEventArgs e)
        {
            if (_EstaAperturando == true)
            {
                RecalculoTotales();
                txtTotal.Text = (totalApertura).ToString("N2");
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(txtTransferencia.Text) && (Convert.ToDecimal(preCierreTransferencia.Text) <= Convert.ToDecimal(txtTransferencia.Text)))
                {
                    preCierreTransferencia.Visible = false;
                }
            }
        }

        private void txtCheque_KeyUp(object sender, KeyEventArgs e)
        {
            if (_EstaAperturando == true)
            {
                RecalculoTotales();
                txtTotal.Text = (totalApertura).ToString("N2");
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(txtCheque.Text) && (Convert.ToDecimal(preCierreCheque.Text) <= Convert.ToDecimal(txtCheque.Text)))
                {
                    preCierreCheque.Visible = false;
                }
            }
        }

        private void txtTarjeta_KeyUp(object sender, KeyEventArgs e)
        {
            if (_EstaAperturando == true)
            {
                RecalculoTotales();
                txtTotal.Text = (totalApertura).ToString("N2");
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(txtTarjeta.Text) && (Convert.ToDecimal(preCierreTarjeta.Text) <= Convert.ToDecimal(txtTarjeta.Text)))
                {
                    preCierreTarjeta.Visible = false;
                }
            }
        }

        private void Limpiar() 
        {
            lblFecha.Text = "-";
            lblEquipo.Text = "-";
            lblUsuario.Text = "-";
            txtCheque.Text = "0.00";
            txtTarjeta.Text = "0.00";
            txtTransferencia.Text = "0.00";
            txtMonto.Text = "0.00";
            txtTotal.Text = "0.00";

            button1.Enabled = false;
            label9.Visible = false;

            //Limpiar Valores Iniciales:
            lblChequeInicial.Text = "0.00";
            lblTarjetaInicial.Text = "0.00";
            lblEfectivoInicial.Text = "0.00";
            lblTransferenciaInicial.Text = "0.00";
            lblTotalInicial.Text = "0.00";
            lblSaldoCaja.Text = "0.00";

            btnProceso.BackColor = Color.FromArgb(97, 172, 112);
            btnProceso.Text = "APERTURAR";

            //Regreso la variable para apertura
            _EstaAperturando = true;

            preCierreTransferencia.Visible = false;
            preCierreTotal.Visible = false;
            preCierreTarjeta.Visible = false;
            preCierreEfectivo.Visible = false;
            preCierreCheque.Visible = false;
        }

        private void txtMonto_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;

            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || e.KeyChar == '.' && !txt.Text.Contains("."))
            {
                return;
            }

            e.Handled = true;
        }
    }
}
