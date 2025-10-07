using Logica;
using ModuloCajaRC.Facturas;
using ModuloCajaRC.LoginMenu;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModuloCajaRC
{
    public partial class DynamicMain : Form
    {
        DataTable tablaEncabezado = new DataTable();
        DataTable dtMenuOpciones = new DataTable();
        DataTable dtSeguimientoUsuario = new DataTable();
        DataTable dtAperturaCaja = new DataTable();
        private static Form activeForm;
        public static RegistroAcciones registro = new RegistroAcciones(); //REGISTRO DE ACCIONES DE USUARIO
        login loginn = new login();
        DataTable dtEstadoENAC = new DataTable();

        public static bool _EstadoEnac = false;
        public static string usuarionombre;
        public static string usuarioNombreCompleto;
        public static string usuarioSucursal;
        public static string usuarioPerfilID;
        public static string rutaEmitirEvento;
        public static string usuarionlogin;
        public static string usuarionEmpresaNombre;
        public static int usuarioIDNumber;
        public static int usuarioEmpleadoID;
        public static int ModuloID;
        public static int usuarioDepartamentoID;
        public static int usuarioSucursalID;
        public static int usuarioEmpresaID;
        public static int usuarioNivelAccesoSolicitud;
        public static int Confidencial;
        public static int cajaID;
        public static bool permisoEditar = false; // variable para poder editar registros / Guardar - Editar - Borrar
        public static bool existeAvisos = false; //Variable para controlar el mostrar o no los avisos.

        bool panelLeftExpand = true;
        clsLogica logica = new clsLogica();
        public static DynamicMain Instance { get; private set; }
        public DynamicMain(string usuario)
        {
            InitializeComponent();
            Instance = this;
            EstadoENAC();
            CargarEncabezado(usuario);
            usuarionombre = usuario;
            usuarionlogin = usuario;
            ModuloID = 17;
            //VERSION

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            string versionStr = $"{versionInfo.ProductMajorPart}.{versionInfo.ProductMinorPart}.{versionInfo.ProductBuildPart}.{versionInfo.ProductPrivatePart}";


            toolStripLabel2.Text = versionStr;
            if (Datos.BD_Conexion.servidor.ToString() == "192.168.1.180")
            {
                rutaEmitirEvento = "http://192.168.1.179:3001";
            }
            else
            {
                rutaEmitirEvento = "https://app.rapidocargo.online:3000";
            }
            toolStripLabel4.Text = Datos.BD_Conexion.servidor.ToString();
            toolStripLabel6.Text = usuarionlogin;

            BuscarMenu();
            CargarMenuDinamico();

            int ancho = Screen.PrimaryScreen.WorkingArea.Width;
            int alto = Screen.PrimaryScreen.WorkingArea.Height;

            this.MaximumSize = new System.Drawing.Size(ancho, alto);
            this.WindowState = FormWindowState.Maximized; // Iniciar maximizado

            flowLayoutPanelMenu.PerformLayout();
            flowLayoutPanelMenu.Refresh();
            flowLayoutPanelMenu.Invalidate();
        }
        private void CargarEncabezado(string usuario)
        {
            tablaEncabezado = loginn.DatosEncabezado(usuario, typeof(Program).Namespace);
            foreach (DataRow row in tablaEncabezado.Rows)
            {
                usuarioNombreCompleto = row["NombreCompleto"].ToString();
                lblUbicacion.Text = "HOME / BIENVENIDO: " + row["NombreCompleto"].ToString();
                //lblUsuarioPerfil.Text = row["Puesto"].ToString();
                //lblEmpresa.Text = row["Sucursal"].ToString();
                toolStripLabel7.Text = Environment.MachineName.ToString();
                usuarioIDNumber = Convert.ToInt32(row["UsuarioID"].ToString());
                permisoEditar = Convert.ToBoolean(row["PermiteEditar"]);
                Confidencial = Convert.ToInt32(row["Confidencial"]);
                usuarioEmpleadoID = Convert.ToInt32(row["EmpleadoID"]);
                usuarioDepartamentoID = Convert.ToInt32(row["DepartamentoID"]);
                usuarioSucursalID = Convert.ToInt32(row["SucursalID"]);
                usuarioSucursal = row["Sucursal"].ToString();
                usuarioPerfilID = row["PerfilID"].ToString();
                usuarioEmpresaID = Convert.ToInt32(row["EmpresaID"]);
                usuarionEmpresaNombre = row["Empresa"].ToString();
            }
        }
        private void BuscarMenu()
        {
            TBLMenuDinamicoLista getTBLMenuDinamicoLista = new TBLMenuDinamicoLista
            {
                Opcion = "ListadoPermiso",
                Valor = "17",
                Valor2 = "1",
                Valor3 = usuarioIDNumber.ToString(),
            };
            try
            {
                dtMenuOpciones = (DataTable)logica.SP_MenuDinamico_GET(getTBLMenuDinamicoLista);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al momento de recuperar el listado de menu, contacte con sistemas." + ex.Message, "Notificacion", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private Dictionary<string, Image> iconDictionary = new Dictionary<string, Image>
        {
             { "frmCaja", global::ModuloCajaRC.Properties.Resources.bill_26px },
        };

        private void CargarMenuDinamico()
        {
            ToolTip toolTip1 = new ToolTip(); // Crear una instancia de ToolTip

            if (dtMenuOpciones.Rows.Count > 0)
            {
                Dictionary<int, FlowLayoutPanel> menuPanels = new Dictionary<int, FlowLayoutPanel>();

                foreach (DataRow row in dtMenuOpciones.Rows)
                {
                    int menuID = Convert.ToInt32(row["MenuID"]);
                    int padreID = Convert.ToInt32(row["PadreID"]);
                    Button btnMenu = new Button();
                    btnMenu.Name = "btn" + row["MenuID"].ToString();
                    btnMenu.Text = "    " + row["Tag"].ToString();
                    btnMenu.Width = 230;
                    btnMenu.Height = 40;
                    btnMenu.FlatStyle = FlatStyle.Flat;
                    btnMenu.Cursor = Cursors.Hand;
                    btnMenu.FlatAppearance.BorderSize = 0;
                    btnMenu.FlatAppearance.MouseDownBackColor = Color.FromArgb(47, 51, 54);
                    btnMenu.ForeColor = System.Drawing.Color.White;
                    btnMenu.Font = new Font("Century Gothic", 9, FontStyle.Bold);
                    btnMenu.FlatAppearance.MouseOverBackColor = Color.FromArgb(141, 153, 163);
                    btnMenu.MouseClick += new MouseEventHandler(BotonMenu_MouseClick);

                    // Cargar el icono desde la carpeta Resources utilizando una ruta absoluta
                    string elemento = row["Descripcion"].ToString();
                    if (!string.IsNullOrEmpty(elemento))
                    {
                        try
                        {
                            if (padreID == 0)
                            {
                                // Icono para el menú principal
                                btnMenu.Image = iconDictionary[elemento];
                                btnMenu.Tag = "collapsed"; // Etiqueta para controlar el estado del menú
                                btnMenu.Margin = new Padding(0); // Sin margen para el primer nivel
                            }
                            else
                            {
                                DataRow[] subMenus = dtMenuOpciones.Select("PadreID = " + menuID);
                                if (subMenus.Length > 0)
                                {
                                    // Icono para los submenús con hijos
                                    btnMenu.Image = global::ModuloCajaRC.Properties.Resources.white_sort_down_16px;
                                }
                                else
                                {
                                    // Icono para los submenús sin hijos
                                    btnMenu.Image = global::ModuloCajaRC.Properties.Resources.white_sort_right_16px;
                                }

                                btnMenu.Margin = new Padding(2, 0, 0, 0); // Márgenes para el segundo nivel
                                if (padreID != 0)
                                {
                                    DataRow[] parentRow = dtMenuOpciones.Select("MenuID = " + padreID);
                                    if (parentRow.Length > 0 && Convert.ToInt32(parentRow[0]["PadreID"]) != 0)
                                    {
                                        // Márgenes para el tercer nivel
                                        btnMenu.Margin = new Padding(2, 0, 0, 0);
                                    }
                                }
                            }

                            btnMenu.ImageAlign = ContentAlignment.MiddleLeft; // Alinea la imagen a la izquierda
                            btnMenu.TextAlign = ContentAlignment.MiddleRight; // Alinea el texto a la derecha
                            btnMenu.TextImageRelation = TextImageRelation.ImageBeforeText; // Coloca el texto después de la imagen
                            btnMenu.Padding = new Padding(10, 0, 0, 0); // Espacio alrededor del botón
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al cargar el icono: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    // Establecer la descripción del ToolTip
                    toolTip1.SetToolTip(btnMenu, row["Tag"].ToString());

                    if (padreID == 0)
                    {
                        FlowLayoutPanel panel = new FlowLayoutPanel();
                        panel.FlowDirection = FlowDirection.TopDown;
                        panel.AutoSize = true;
                        panel.Controls.Add(btnMenu);

                        flowLayoutPanelMenu.Controls.Add(panel);
                        menuPanels.Add(menuID, panel);
                    }
                    else
                    {
                        if (menuPanels.ContainsKey(padreID))
                        {
                            btnMenu.Visible = false;  // Los submenús están ocultos por defecto
                            btnMenu.TextAlign = ContentAlignment.MiddleLeft; // Alinea el texto del submenú a la izquierda

                            FlowLayoutPanel subPanel;
                            if (!menuPanels.ContainsKey(menuID))
                            {
                                subPanel = new FlowLayoutPanel();
                                subPanel.FlowDirection = FlowDirection.TopDown;
                                subPanel.AutoSize = true;
                                subPanel.Margin = new Padding(3, 0, 0, 0);
                                subPanel.Visible = false;
                                menuPanels.Add(menuID, subPanel);
                                menuPanels[padreID].Controls.Add(subPanel);
                            }
                            else
                            {
                                subPanel = menuPanels[menuID];
                            }

                            subPanel.Controls.Add(btnMenu);
                        }
                    }
                }
            }
        }
        private void BotonMenu_MouseClick(object sender, MouseEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                int menuID = Convert.ToInt32(clickedButton.Name.Replace("btn", ""));
                bool isParent = dtMenuOpciones.AsEnumerable().Any(row => Convert.ToInt32(row["MenuID"]) == menuID && Convert.ToInt32(row["PadreID"]) == 0);
                bool hasChild = dtMenuOpciones.AsEnumerable().Any(row => Convert.ToInt32(row["PadreID"]) == menuID && Convert.ToInt32(row["PadreID"]) != 0);

                if (isParent && !hasChild)  // Si es "Padre" pero NO tiene hijos, abre directamente el formulario
                {
                    switch (clickedButton.Text.Trim())
                    {
                        
                        default:
                            break;
                    }
                }
                if (isParent)
                {
                    // Alternar la visibilidad del panel de submenús de segundo nivel
                    foreach (Control control in flowLayoutPanelMenu.Controls)
                    {
                        if (control is FlowLayoutPanel panel && panel.Controls.Contains(clickedButton))
                        {
                            bool shouldExpand = clickedButton.Tag == null || clickedButton.Tag.ToString() == "collapsed";
                            ToggleSubMenuVisibility(panel, clickedButton, shouldExpand, 1);
                            clickedButton.Tag = shouldExpand ? "expanded" : "collapsed";
                        }
                    }
                }
                else
                {
                    if (hasChild)
                    {
                        bool shouldExpand = clickedButton.Tag == null || clickedButton.Tag.ToString() == "collapsed";

                        foreach (DataRow dRow in dtMenuOpciones.Rows)
                        {
                            if (Convert.ToInt32(dRow["PadreID"]) == menuID)
                            {
                                FlowLayoutPanel parentPanel = clickedButton.Parent as FlowLayoutPanel;

                                foreach (Control control in parentPanel.Controls)
                                {
                                    if (control is FlowLayoutPanel subPanel)
                                    {
                                        foreach (Control subControl in subPanel.Controls)
                                        {
                                            subControl.Visible = shouldExpand;
                                        }
                                        subPanel.Visible = shouldExpand;
                                    }
                                    else if (control != clickedButton)
                                    {
                                        control.Visible = shouldExpand;
                                    }
                                }
                                clickedButton.Tag = shouldExpand ? "expanded" : "collapsed";
                            }
                        }
                    }
                    else
                    {
                        // Si no tiene hijos, abrir el formulario correspondiente
                        switch (clickedButton.Text.Trim())
                        {
                            case "FACTURAS":
                                SeguimientoUsuario(44); //44 ESTA DEFINIDO COMO - INGRESA A frmConsultarClientes
                                AperturarCaja();
                                
                                break;
                            case "CAJA - APERTURA":
                                SeguimientoUsuario(44); //44 ESTA DEFINIDO COMO - INGRESA A frmConsultarClientes
                                LanzarForm(new frmApertura(), "HOME / APERTURA");
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        private void AperturarCaja()
        {
            ControlCajaDTO sendApertura = new ControlCajaDTO
            {
                Opcion = "AperturaCaja",
                Estado = true,
                UPosteo = DynamicMain.usuarionlogin,
                PC = System.Environment.MachineName
            };
            dtAperturaCaja = logica.SP_ControlCaja(sendApertura);
            if (dtAperturaCaja.Rows.Count > 0)
            {
                cajaID = Convert.ToInt32(dtAperturaCaja.Rows[0]["ControlID"].ToString());
                LanzarForm(new frmFacturasGeneral(), "HOME / FACTURAS");
            }
            else
            {
                MessageBox.Show("Aun no ha aperturado la caja.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void ActiveForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ActualizarUbicacion("HOME / BIENVENIDO: " + usuarioNombreCompleto);
        }
        private void ActualizarUbicacion(string ubicacionLabel)
        {
            // Suspender el diseño del formulario temporalmente
            this.SuspendLayout();
            lblUbicacion.Text = ubicacionLabel;
            lblUbicacion.Refresh();
            // Reanudar el diseño del formulario
            this.ResumeLayout();
        }
        private void EnableDoubleBuffering(Form form)
        {
            form.GetType().InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.SetProperty,
                null, form, new object[] { true });
        }
        public void LanzarForm(Form childForm, string ubicacionLabel)
        {
            if (activeForm != null)
            {
                // Suscribirse al evento FormClosed del formulario activo
                activeForm.FormClosed -= ActiveForm_FormClosed; // Desuscribirse para evitar múltiples suscripciones
                activeForm.Close();
            }
            activeForm = childForm;
            activeForm.FormClosed += ActiveForm_FormClosed; // Suscribirse al evento FormClosed

            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            // Habilitar doble búfer para reducir parpadeo
            EnableDoubleBuffering(childForm);

            this.pWorkspace.SuspendLayout(); // Suspender el diseño del panel principal
            this.pWorkspace.Controls.Add(childForm);
            this.pWorkspace.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            this.pWorkspace.ResumeLayout(); // Reanudar el diseño del panel principal

            // Actualizar el lblUbicacion con el valor proporcionado
            ActualizarUbicacion(ubicacionLabel);
        }
        public void SeguimientoUsuario(int _AccionID)
        {
            SeguimientoUsuario sendSeguimiento = new SeguimientoUsuario
            {
                Operacion = "INSERTAR",
                Usuario = DynamicMain.usuarionlogin,
                Modulo = Assembly.GetExecutingAssembly().GetName().Name,
                Formulario = this.Name,
                AccionID = _AccionID,
                UPosteo = DynamicMain.usuarionlogin,
                FPosteo = DateTime.Now,
                PC = System.Environment.MachineName,
                Estado = true
            };
            dtSeguimientoUsuario = logica.SP_SeguimientoUsuario(sendSeguimiento);
        }
        private void ToggleSubMenuVisibility(FlowLayoutPanel parentPanel, Button clickedButton, bool shouldExpand, int nivel)
        {
            foreach (Control subControl in parentPanel.Controls)
            {
                if (subControl is FlowLayoutPanel subPanel)
                {
                    foreach (Control subSubControl in subPanel.Controls)
                    {
                        if (subSubControl is Button subButton)
                        {
                            int subMenuID = Convert.ToInt32(subButton.Name.Replace("btn", ""));
                            bool isSubMenu = dtMenuOpciones.AsEnumerable().Any(row => Convert.ToInt32(row["PadreID"]) == subMenuID);
                            if (nivel < 2 && isSubMenu)
                            {
                                FlowLayoutPanel subSubPanel = subButton.Parent as FlowLayoutPanel;
                                ToggleSubMenuVisibility(subSubPanel, subButton, shouldExpand, nivel + 1);
                            }
                            subButton.Visible = shouldExpand && nivel < 2;
                        }
                    }
                    subPanel.Visible = shouldExpand;
                }
                else if (subControl != clickedButton)
                {
                    subControl.Visible = shouldExpand && nivel < 2;
                }
            }
        }
        private void PanelLeftTimer_Tick(object sender, EventArgs e)
        {
            if (panelLeftExpand)
            {
                pLeftMenu.Width -= 10;
                if (pLeftMenu.Width <= pLeftMenu.MinimumSize.Width)
                {
                    panelLeftExpand = false;
                    PanelLeftTimer.Stop();
                }
            }
            else
            {
                pLeftMenu.Width += 10;
                if (pLeftMenu.Width >= pLeftMenu.MaximumSize.Width)
                {
                    panelLeftExpand = true;
                    PanelLeftTimer.Stop();
                }
            }
        }

        private void pbxMenu_Click(object sender, EventArgs e)
        {
            if (!PanelLeftTimer.Enabled)
            {
                if (pLeftMenu.Width == pLeftMenu.MaximumSize.Width)
                {
                    panelLeftExpand = true;
                }
                else if (pLeftMenu.Width == pLeftMenu.MinimumSize.Width)
                {
                    panelLeftExpand = false;
                }

                PanelLeftTimer.Start();
            }
        }
        private void EstadoENAC()
        {
            dtEstadoENAC.Clear();
            EstadoENAC getEstado = new EstadoENAC()
            {
                Opcion = "Listado",
                ID = 1
            };
            dtEstadoENAC = logica.SP_EstadoENAC(getEstado);
            if (dtEstadoENAC.Rows.Count > 0)
            {
                _EstadoEnac = Convert.ToBoolean(dtEstadoENAC.Rows[0]["Estado"]);

                toolStripLabel10.Text = (_EstadoEnac) ? "ACTIVO" : "INACTIVO";
            }
        }
        private void CargarTema()
        {
            pLeftMenuLogo.BackColor = Color.FromArgb(236, 240, 241);
            pLeftMenu.BackColor = Color.FromArgb(84, 102, 118);
            pHeaderMain.BackColor = Color.FromArgb(84, 102, 118);
            pFooter.BackColor = Color.FromArgb(84, 102, 118);
            pHeaderOptions.BackColor = Color.FromArgb(240, 240, 240);
        }

        private void DynamicMain_Load(object sender, EventArgs e)
        {
            CargarTema();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Logout();
        }
        private void Logout()
        {
            DialogResult result = MessageBox.Show("¿Desea cerrar sesion? Recuerda guardar todo tu progreso.", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Hide(); // Ocultar el formulario actual
                var loginForm = new Login();
                loginForm.Show();
            }
        }

        private void DynamicMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) // Si el usuario cierra con la "X"
            {
                DialogResult result = MessageBox.Show("¿Está seguro de que desea salir del sistema?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true; // Cancela el cierre si el usuario elige "No"
                }
                else
                {
                    Application.Exit(); // Cierra toda la aplicación
                }
            }
        }
    }
}
