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
    public partial class frmComentariocCierreCaja : Form
    {
        public  string textoCierre = "";
        public  bool ingresoAlgo = false;
        
        public frmComentariocCierreCaja()
        {
            InitializeComponent();
        }

        private void frmComentariocCierreCaja_Load(object sender, EventArgs e)
        {
            
        }

        private void pbxClose_Click(object sender, EventArgs e)
        {
            textoCierre = txtTextoCierre.Text;
            this.Close();
        }

        private void txtTextoCierre_KeyPress(object sender, KeyPressEventArgs e)
        {
            ingresoAlgo = (!String.IsNullOrEmpty(txtTextoCierre.Text)) ? true : false;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            textoCierre = txtTextoCierre.Text;
            this.Close();
        }
    }
}
