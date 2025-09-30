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

namespace ModuloFacturacionRC.Facturas
{
    public partial class frmApertura : Form
    {
        DataTable dtAperturaCaja = new DataTable();
        clsLogica logica = new clsLogica();
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
            CajaAperturaDTO sendApertura = new CajaAperturaDTO 
            {
                Opcion = "Listado",
                UPosteo = DynamicMain.usuarionlogin,
                PC = System.Environment.MachineName
            };
            dtAperturaCaja = logica.SP_CajaApertura(sendApertura);
            if (dtAperturaCaja.Rows.Count > 0) 
            {
                txtMonto.ReadOnly = true;
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
            CajaAperturaDTO sendApertura = new CajaAperturaDTO
            {
                Opcion = "Agregar",
                Monto = Convert.ToDecimal(txtMonto.Text),
                UPosteo = DynamicMain.usuarionlogin,
                FPosteo = DateTime.Now,
                PC = System.Environment.MachineName,
                Estado = true
            };
            dtAperturaCaja = logica.SP_CajaApertura(sendApertura);
            if (dtAperturaCaja.Rows.Count > 0 && dtAperturaCaja.Rows[0]["Estado"].ToString() == "1")
            {
                MessageBox.Show("Apertura de caja realizada exitosamente!","Notificacion",MessageBoxButtons.OK,MessageBoxIcon.Information);
                Limpiar();
            }
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

        private void button1_Click(object sender, EventArgs e)
        {
            EnviarAperturaCaja();
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
