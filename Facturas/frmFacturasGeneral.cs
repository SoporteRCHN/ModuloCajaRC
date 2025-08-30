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

namespace ModuloFacturacionRC.Facturas
{
    public partial class frmFacturasGeneral : Form
    {
        DataTable dtPermisos = new DataTable();
        DataTable dtFacturas = new DataTable();
        DataTable dtMetodoPago = new DataTable();
        clsLogica logica = new clsLogica();

        private bool usuarioInteractuando = false;

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
                    lblTotal.Text = dgvFacturas.Rows[filaActual].Cells["Total"].Value?.ToString() ?? "L. 0.00";
                }
                else
                {
                    dgvFacturas.CurrentCell = dgvFacturas.Rows[0].Cells["Total"];
                    lblTotal.Text = dgvFacturas.Rows[0].Cells["Total"].Value?.ToString() ?? "L. 0.00";
                }
            }
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
                    dgvMetodosPago.Rows.Add(row["MetodoID"], row["Descripcion"],"");
                }
                AjustarAlturaFilas();
            }
        }
        private void AjustarAlturaFilas()
        {
            int totalFilas = dgvMetodosPago.Rows.Count;

            if (totalFilas > 0)
            {
                // Restamos el alto del encabezado para calcular solo el área de filas
                int alturaDisponible = dgvMetodosPago.ClientSize.Height - dgvMetodosPago.ColumnHeadersHeight;

                // Calculamos la altura ideal por fila
                int alturaPorFila = alturaDisponible / totalFilas;

                // Asignamos la altura a cada fila
                foreach (DataGridViewRow fila in dgvMetodosPago.Rows)
                {
                    fila.Height = alturaPorFila;
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
                                var resourceManager = ModuloFacturacionRC.Properties.Resources.ResourceManager;
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
                                    //DynamicMain.Instance.LanzarForm(new frmEditarClientes(UbicacionActual), "HOME / REGISTRO DE CLIENTES");
                                    MessageBox.Show("Registro procesado correctamente","Notificacion",MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                lblTotal.Text = dgvFacturas.CurrentRow.Cells["Total"].Value.ToString();
            }
        }
        private void Limpiar() 
        {
            foreach (DataGridViewRow row in dgvMetodosPago.Rows)
            {
                if (row.Cells["mValor"].Value != null) 
                {
                    row.Cells["mValor"].Value = "";
                }
            }

            lblTotal.Text = "0.00";
            lblRecibido.Text = "0.00";
            lblCambio.Text = "0.00";
        }

        private void dgvFacturas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            usuarioInteractuando = true;

            if (e.RowIndex >= 0 && dgvFacturas.Rows[e.RowIndex].Cells["Total"].Value != null)
            {
                lblTotal.Text = dgvFacturas.Rows[e.RowIndex].Cells["Total"].Value.ToString();
            }

            Task.Delay(1000).ContinueWith(_ =>
            {
                usuarioInteractuando = false;
            });
        }

        private void dgvMetodosPago_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvMetodosPago.Columns[e.ColumnIndex].Name == "mValor")
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
                if (fila.Cells["mValor"].Value != null &&
                    decimal.TryParse(fila.Cells["mValor"].Value.ToString(), out decimal valor))
                {
                    total += valor;
                }
            }
            lblCambio.Text =  (total > Convert.ToDecimal(lblTotal.Text)) ? (total - Convert.ToDecimal(lblTotal.Text)).ToString() : "0.00";
            lblRecibido.Text = $"{total:N2}";

        }

    }
}
