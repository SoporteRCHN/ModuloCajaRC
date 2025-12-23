using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using LogicaCaja;
using ModuloCajaRC.Facturas;
using ModuloCajaRC.LoginMenu;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Windows.Forms;

namespace ModuloCajaRC
{
    public partial class DynamicMain : Form
    {
        DataTable tablaEncabezado = new DataTable();
        DataTable dtMenuOpciones = new DataTable();
        DataTable dtTasaCambio = new DataTable();
        DataTable dtTasaCambioHistorico = new DataTable();
        DataTable dtSeguimientoUsuario = new DataTable();
        DataTable dtContingencias = new DataTable();
        DataTable dtAperturaCaja = new DataTable();
        DataTable dtMenuOpcionesFinal = new DataTable();
        DataTable dtEstadoENAC = new DataTable();

        private static Form activeForm;
        public static RegistroAcciones registro = new RegistroAcciones(); //REGISTRO DE ACCIONES DE USUARIO
        login loginn = new login();
       
        public static bool _EstadoEnac = false;
        public static string usuarionombre;
        public static string usuarioNombreCompleto;
        public static string usuarioSucursal;
        public static string usuarioPerfilID;
        public static string rutaEmitirEvento, _Origen;
        public static string usuarionlogin;
        public static string usuarionEmpresaNombre;
        public static int usuarioIDNumber;
        public static int usuarioEmpleadoID;
        public static int ModuloID;
        public static int usuarioDepartamentoID;
        public static int usuarioSucursalID;
        public static int usuarioCiudadID;
        public static int usuarioEmpresaID;
        public static int usuarioNivelAccesoSolicitud;
        public static int usuarioSucursalCaja;
        public static int usuarioAutorizaCierreCaja;
        public static int Confidencial;
        public static int cajaID; //Ayuda en el manejo del ControlID cuando se apertura caja
        public static decimal tasa;
        public static bool permisoEditar = false; // variable para poder editar registros / Guardar - Editar - Borrar
        public static bool existeAvisos = false; //Variable para controlar el mostrar o no los avisos.
        public static bool ContingenciaBodega = false;

        bool panelLeftExpand = true;
        clsLogica logica = new clsLogica();
        public static DynamicMain Instance { get; private set; }
        public DynamicMain(string usuario, string Origen)
        {
            InitializeComponent();
            Instance = this;
            usuarionombre = usuario;
            usuarionlogin = usuario;
            _Origen = Origen;
            ModuloID = 22;

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            string versionStr = $"{versionInfo.ProductMajorPart}.{versionInfo.ProductMinorPart}.{versionInfo.ProductBuildPart}.{versionInfo.ProductPrivatePart}";
            toolStripLabel2.Text = versionStr;

            if (DatosCaja.BD_Conexion.servidor.ToString() == "192.168.1.180")
            {
                rutaEmitirEvento = "http://192.168.1.179:3001";
            }
            else
            {
                rutaEmitirEvento = "https://app.rapidocargo.online:3000";
            }

            toolStripLabel4.Text = DatosCaja.BD_Conexion.servidor.ToString();
            toolStripLabel6.Text = usuarionlogin;

            // Llamar método async separado
            EstadoENAC();
            CargarEncabezado(usuarionlogin);
            RecuperarContingencias();

            BuscarMenuFinal();

            tasa = ActualizarTasaCambio();
            lblTasa.Text = tasa.ToString();
            lblFechaActual.Text = DateTime.Today.ToString("dd/MM/yyyy");

            int ancho = Screen.PrimaryScreen.WorkingArea.Width;
            int alto = Screen.PrimaryScreen.WorkingArea.Height;

            this.MaximumSize = new System.Drawing.Size(ancho, alto);
            this.WindowState = FormWindowState.Maximized;

            flowLayoutPanelMenu.PerformLayout();
            flowLayoutPanelMenu.Refresh();
            flowLayoutPanelMenu.Invalidate();
        }
        private void BuscarMenuFinal()
        {
            // 1. Perfil
            PerfilPermisoDTO perfil = new PerfilPermisoDTO
            {
                Opcion = "ListadoPorPerfil",
                PerfilID = Convert.ToInt32(usuarioPerfilID),
                PerfilPermisoID = ModuloID
            };
            DataTable dtPerfil = logica.SP_PerfilPermisos(perfil);

            // 2. Extras
            PerfilPermisosExtraDTO extra = new PerfilPermisosExtraDTO
            {
                Opcion = "ListadoPorUsuario",
                UsuarioID = usuarioIDNumber,
                PermisoExtraID = ModuloID
            };
            DataTable dtExtra = logica.SP_PerfilPermisosExtra(extra);

            // 3. Merge
            dtMenuOpcionesFinal = CombinarMenus(dtPerfil, dtExtra);

            if (dtMenuOpcionesFinal.Rows.Count > 0)
            {
                CargarMenuDinamico(dtMenuOpcionesFinal);
            }
        }
        private DataTable CombinarMenus(DataTable dtPerfil, DataTable dtExtra)
        {
            // Clonar estructura del perfil
            DataTable dtFinal = dtPerfil.Clone();

            // Copiar todos los registros del perfil
            foreach (DataRow row in dtPerfil.Rows)
            {
                dtFinal.ImportRow(row);
            }

            // Aplicar overlay de extras
            foreach (DataRow extra in dtExtra.Rows)
            {
                int menuID = Convert.ToInt32(extra["MenuID"]);
                bool estadoExtra = Convert.ToBoolean(extra["Estado"]);

                // Buscar si ya existe en el perfil
                DataRow[] existentes = dtFinal.Select("MenuID = " + menuID);

                if (existentes.Length > 0)
                {
                    if (!estadoExtra)
                    {
                        // Quitar del menú final
                        foreach (DataRow r in existentes)
                            dtFinal.Rows.Remove(r);
                    }
                    else
                    {
                        // Sobreescribir propiedades
                        DataRow r = existentes[0];
                        r["Estado"] = extra["Estado"];
                    }
                }
                else
                {
                    if (estadoExtra)
                    {
                        // Agregar nuevo registro exclusivo del usuario
                        dtFinal.ImportRow(extra);
                    }
                }
            }

            // Ordenar por PadreID y luego MenuID
            DataView dv = dtFinal.DefaultView;
            dv.Sort = "PadreID ASC, MenuID ASC";
            return dv.ToTable();
        }
        private void CargarMenuDinamico(DataTable dtMenuOpciones)
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
                    btnMenu.Text = row["Tag"].ToString();
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
        public void SeguimientoUsuario(string _Operacion, int _AccionID)
        {
            SeguimientoUsuario sendSeguimiento = new SeguimientoUsuario
            {
                Operacion = _Operacion,
                Usuario = usuarionlogin,
                Modulo = Assembly.GetExecutingAssembly().GetName().Name,
                Formulario = this.Name,
                AccionID = _AccionID,
                UPosteo = usuarionlogin,
                FPosteo = DateTime.Now,
                PC = System.Environment.MachineName,
                Estado = true

            };
            dtSeguimientoUsuario = logica.SP_SeguimientoUsuario(sendSeguimiento);
        }
        private decimal ActualizarTasaCambio() 
        {
            FactorDolar50DTO setTasa = new FactorDolar50DTO
            {
                Opcion = "Recuperar"
            };
            dtTasaCambio = logica.SP_FactorDolar50(setTasa);
            if (dtTasaCambio.Rows.Count > 0) 
            {
                
                return Convert.ToDecimal(dtTasaCambio.Rows[0]["FactorDolar"].ToString());
            }
            else 
            {
                MessageBox.Show("Ocurrio un problema al actualizar la tasa de cambio en la base de datos", "Aviso Urgente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
        private void ActualizarTasaCambioHistorico(decimal tasa) 
        {
            FactorDolarHistorico50DTO setTasaHistorico = new FactorDolarHistorico50DTO()
            {
                Opcion = "Agregar",
                FactorDolar = tasa,
                UPosteo = DynamicMain.usuarionlogin,
                FPosteo = DateTime.Now,
                PC = Environment.MachineName,
                Estado = true
            };

            dtTasaCambioHistorico = logica.SP_FactorDolarHistorico50(setTasaHistorico);

            if (dtTasaCambioHistorico.Rows.Count > 0 && dtTasaCambioHistorico.Rows[0]["Estado"] == "0")
            {
                MessageBox.Show("Ocurrio un problema al actualizar la tasa de cambio en la base de datos", "Aviso Urgente", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static async Task<decimal> TasaDeCambioAsync()
        {

            DateTime fecha = DateTime.UtcNow;
            
            if (fecha.DayOfWeek == DayOfWeek.Saturday) 
            {
                fecha = fecha.AddDays(2);
            }
            else if (fecha.DayOfWeek == DayOfWeek.Sunday)
            {
                fecha = fecha.AddDays(1);
            }
            
            string codigo = "620";
            string clave = "4354437e7fd3475090c6c739d3276af8";

            string url = $"https://bchapi-am.azure-api.net/api/v1/indicadores/{codigo}/cifras?formato=Json&fechaInicio={fecha.ToString("yyyy-MM-dd")}&fechaFinal={fecha.ToString("yyyy-MM-dd")}";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
                client.DefaultRequestHeaders.Add("clave", clave);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    JArray data = JArray.Parse(responseBody);

                    // Convertir a decimal
                    decimal tasaCambio = Convert.ToDecimal(data[0]["Valor"]);

                    Console.WriteLine("Tasa de cambio recibida: " + tasaCambio);
                    return tasaCambio;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return 0m; // Valor por defecto en caso de error
                }
            }
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
                usuarioSucursalCaja = Convert.ToInt32(row["CajaActiva"]);
                usuarioAutorizaCierreCaja = Convert.ToInt32(row["AutorizaCierreCaja"]);
                toolStripLabel12.Text = row["Sucursal"].ToString();
            }
        }

        private void RecuperarContingencias()
        {
            PlanContingenciaDTO getContingencia = new PlanContingenciaDTO 
            {
                Opcion = "Recuperar",
                Descripcion = "ContingenciaBodega",
                SucursalID = DynamicMain.usuarioSucursalID
            };
            dtContingencias = logica.SP_PlanContingencia(getContingencia);
            if(dtContingencias.Rows.Count>0) 
            {
                ContingenciaBodega = Convert.ToBoolean(dtContingencias.Rows[0]["Estado"].ToString());
            }
        }



        private Dictionary<string, Image> iconDictionary = new Dictionary<string, Image>
        {
             { "frmCaja", global::ModuloCajaRC.Properties.Resources.bill_26px },
        };

    
        private void BotonMenu_MouseClick(object sender, MouseEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                int menuID = Convert.ToInt32(clickedButton.Name.Replace("btn", ""));
                bool isParent = dtMenuOpcionesFinal.AsEnumerable().Any(row => Convert.ToInt32(row["MenuID"]) == menuID && Convert.ToInt32(row["PadreID"]) == 0);
                bool hasChild = dtMenuOpcionesFinal.AsEnumerable().Any(row => Convert.ToInt32(row["PadreID"]) == menuID && Convert.ToInt32(row["PadreID"]) != 0);

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
                                if (cajaID != 0) 
                                {
                                    SeguimientoUsuario(44); //44 ESTA DEFINIDO COMO - INGRESA A frmConsultarClientes
                                    LanzarForm(new frmFacturasGeneral(), "HOME / FACTURAS");
                                }
                                else
                                {
                                    MessageBox.Show("Debe iniciar la caja primero.","Aviso",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                                }
                                    
                                break;
                            case "CAJA CONTROL":
                                    SeguimientoUsuario(44); //44 ESTA DEFINIDO COMO - INGRESA A frmConsultarClientes

                                if (cajaID != 0) 
                                {
                                    LanzarForm(new frmApertura(), "HOME / CIERRE DE CAJA");
                                }
                                else
                                {
                                    LanzarForm(new frmApertura(), "HOME / APERTURA DE CAJA");
                                }
                                   
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
                PC = System.Environment.MachineName,
                SucursalID = DynamicMain.usuarioSucursalID
            };
            dtAperturaCaja = logica.SP_ControlCaja(sendApertura);
            if (dtAperturaCaja.Rows.Count > 0)
            {
                cajaID = Convert.ToInt32(dtAperturaCaja.Rows[0]["ControlID"].ToString());
            }
        }
        private void ActiveForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ActualizarUbicacion("HOME / BIENVENIDO: " + usuarioNombreCompleto);
        }
        public void ActualizarUbicacion(string ubicacionLabel)
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
            if (_Origen == "Lanzador")
            {
                DynamicMain.Instance.SeguimientoUsuario("INSERTAR", 42);
                this.Close();
            }
            else if (_Origen == "Independiente")
            {
                DialogResult result = MessageBox.Show("¿Desea cerrar sesion? Recuerda guardar todo tu progreso.", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    DynamicMain.Instance.SeguimientoUsuario("INSERTAR", 42);
                    this.Hide(); // Ocultar el formulario actual
                    var loginForm = new Login();
                    loginForm.Show();
                }
            }
        }
        private void DynamicMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) // Si el usuario cierra con la "X"
            {
                if (_Origen == "Lanzador")
                {
                    DialogResult resultLanzador = MessageBox.Show("¿Desea salir del modulo actual? Recuerda guardar todo lo que tengas abierto.", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (resultLanzador == DialogResult.Yes)
                    {
                        DynamicMain.Instance.SeguimientoUsuario("INSERTAR", 42);
                        this.Dispose();
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
                else if (_Origen == "Independiente")
                {
                    DialogResult resultCierre = MessageBox.Show("¿Está seguro de que desea salir del sistema?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (resultCierre == DialogResult.Yes)
                    {
                        DynamicMain.Instance.SeguimientoUsuario("INSERTAR", 41);
                        Application.Exit(); // Cierra toda la aplicación
                    }
                    else { e.Cancel = true; }
                }
            }
        }

        private void DynamicMain_Shown(object sender, EventArgs e)
        {
            AperturarCaja();
        }
    }
}

