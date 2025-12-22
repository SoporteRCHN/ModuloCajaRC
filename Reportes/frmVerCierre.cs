using LogicaCaja;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModuloCajaRC.Reportes
{
    public partial class frmVerCierre : Form
    {
        public frmVerCierre(string ControlID, string Sucursal, string Equipo)
        {
            InitializeComponent();
            GenerarReporte(ControlID, Sucursal, Equipo);
        }

        private void frmVerCierre_Load(object sender, EventArgs e)
        {

        }
        private void GenerarReporte(string ControlID, string Sucursal, string Equipo)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Remote;
            reportViewer1.ServerReport.ReportServerUrl = new Uri("http://192.168.1.179/ReportServer");
            reportViewer1.ServerReport.ReportPath = "/Reportes/ReportesDeCaja/reporteCierreDeCaja"; 

            ReportParameter[] parameters = new ReportParameter[3];
            parameters[0] = new ReportParameter("ControlID", ControlID);
            parameters[1] = new ReportParameter("Sucursal", Sucursal);
            parameters[2] = new ReportParameter("Equipo", Equipo);
            

            reportViewer1.ServerReport.SetParameters(parameters);
            reportViewer1.RefreshReport();
        }
    }
}
