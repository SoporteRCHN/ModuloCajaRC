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
        clsLogica logica = new clsLogica();

        decimal _Efectivo, _Cheque, _Transferencia, _Tarjeta, _Total = 0;
        public frmApertura()
        {
            InitializeComponent();
            AperturarCaja();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void AperturarCaja() 
        {
            ControlCajaDTO sendApertura = new ControlCajaDTO
            {
                Opcion = "Listado",
                UPosteo = DynamicMain.usuarionlogin,
                PC = System.Environment.MachineName
            };
            dtAperturaCaja = logica.SP_ControlCaja(sendApertura);
            if (dtAperturaCaja.Rows.Count > 0) 
            {
                lblFecha.Text = dtAperturaCaja.Rows[0]["FPosteo"].ToString();
                lblEquipo.Text = dtAperturaCaja.Rows[0]["PC"].ToString();
                lblUsuario.Text = dtAperturaCaja.Rows[0]["NombreCompleto"].ToString();
                txtMonto.Text = Convert.ToDecimal(dtAperturaCaja.Rows[0]["MontoEfectivo"]).ToString("N2");
                txtCheque.Text = "0.00";
                txtTarjeta.Text = "0.00";
                txtTransferencia.Text = "0.00";
                txtTotal.Text = Convert.ToDecimal(dtAperturaCaja.Rows[0]["MontoEfectivo"]).ToString("N2");

                txtMonto.ReadOnly = true;
                txtCheque.ReadOnly = true;
                txtTarjeta.ReadOnly = true;
                txtTotal.ReadOnly = true;
                txtTransferencia.ReadOnly = true;

                button1.Enabled = false;
                label9.Text = "Ya existe una apertura de caja realizada.";
                label9.Visible = true;
            }
            else
            {
                lblFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
                lblEquipo.Text = System.Environment.MachineName;
                lblUsuario.Text = DynamicMain.usuarionlogin;
            }
        }
        private void EnviarAperturaCaja()
        {
            ControlCajaDTO sendApertura = new ControlCajaDTO
            {
                Opcion = "Agregar",
                TipoID = 1, //ID para Apertura
                MontoCheque = (!String.IsNullOrWhiteSpace(txtCheque.Text)) ? Convert.ToDecimal(txtCheque.Text) : 0,
                MontoEfectivo = Convert.ToDecimal(txtMonto.Text),
                MontoTarjeta = (!String.IsNullOrWhiteSpace(txtTarjeta.Text)) ? Convert.ToDecimal(txtTarjeta.Text) : 0,
                MontoTransferencia = (!String.IsNullOrWhiteSpace(txtTransferencia.Text)) ? Convert.ToDecimal(txtTransferencia.Text) : 0,
                MontoTotal = (!String.IsNullOrWhiteSpace(txtTotal.Text)) ? Convert.ToDecimal(txtTransferencia.Text) : 0,
                UPosteo = DynamicMain.usuarionlogin,
                FPosteo = DateTime.Now,
                PC = System.Environment.MachineName,
                Estado = true
            };
            dtAperturaCaja = logica.SP_ControlCaja(sendApertura);
            if (dtAperturaCaja.Rows.Count > 0 && dtAperturaCaja.Rows[0]["Estado"].ToString() == "1")
            {
                MessageBox.Show("Apertura de caja realizada exitosamente!","Notificacion",MessageBoxButtons.OK,MessageBoxIcon.Information);
                Limpiar();
            }
        }
        private void txtMonto_KeyUp(object sender, KeyEventArgs e)
        {
            _Total = (!String.IsNullOrWhiteSpace(txtMonto.Text)) ? Convert.ToDecimal(txtMonto.Text) : 0;
            txtTotal.Text = _Total.ToString("N2");
        }

        private void frmApertura_Load(object sender, EventArgs e)
        {

        }

        private void btnProceso_Click(object sender, EventArgs e)
        {
            EnviarAperturaCaja();
        }

        private void txtTarjeta_KeyUp(object sender, KeyEventArgs e)
        {
            _Tarjeta = (!String.IsNullOrWhiteSpace(txtMonto.Text)) ? Convert.ToDecimal(txtMonto.Text) : 0;
            txtTotal.Text = (_Total+ _Tarjeta).ToString("N2");
        }

        private void Limpiar() 
        {
            lblFecha.Text = "-";
            lblEquipo.Text = "-";
            lblUsuario.Text = "-";
            txtMonto.Text = String.Empty;
            txtMonto.ReadOnly = true;
            button1.Enabled = false;
            label9.Visible = false;
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
