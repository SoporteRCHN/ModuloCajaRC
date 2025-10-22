﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Datos
{
    public class EnviarConsultas
    {
        private BD_Conexion Conexion = new BD_Conexion();

        SqlDataReader leer = null;
        SqlCommand comando = new SqlCommand();
        SqlDataAdapter adaptador = new SqlDataAdapter();
        SqlParameter parametro = new SqlParameter();

        public DataTable SP_Impresoras(dynamic a)
        {

            comando.Parameters.Clear();
            DataTable tabla = new DataTable();

            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            tabla = new DataTable();
            comando.Connection = Conexion.AbrirConexion(3);

            comando.Parameters.AddWithValue("@Opcion", a.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ImpresoraID", a.ImpresoraID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Descripcion", a.Descripcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Tipo", a.Tipo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", a.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", a.FPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", a.PC ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Estado", a.Estado ?? (object)DBNull.Value);

            comando.CommandText = "RCCONFIG.Empresa.SP_Impresoras";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);
            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable CargarDatos(int Usuario, int OpcionSP, int OpcionCampo, string valor)
        {
            DataTable tabla = new DataTable();
            comando.Parameters.Clear();


            // Diccionario para mapear opciones a nombres de procedimientos almacenados
            Dictionary<int, Tuple<string, int>> procedimientos = new Dictionary<int, Tuple<string, int>>
                {
                  { 1,Tuple.Create("RCRH.Empleados.SP_ObtenerEmpleadoGeneral",3)},
                  { 2, Tuple.Create("RCCONFIG.Empresa.SP_ObtenerPaises",2)},
                  { 3, Tuple.Create("RCRH.Empleados.SP_ObtenerEmpleadosIngresosEgresos",1)},
                  { 4, Tuple.Create("RCRH.Empleados.SP_ObtenerEstadoCivil",3)},
                  { 5,Tuple.Create ("RCRH.Empleados.SP_ObtenerTipoSangre",1)},
                  { 6,Tuple.Create( "RCCONFIG.Empresa.SP_ObtenerCiudades",2)},
                  { 7, Tuple.Create("RCCONFIG.Empresa.SP_ObtenerSucursales",2)},
                  { 8, Tuple.Create("RCCONFIG.Empresa.SP_ObtenerDepartamentosEmpresa",2)},
                  { 9, Tuple.Create("RCCONFIG.Empresa.SP_ObtenerEmpleadosPuestos",2)},
                  { 10,Tuple.Create( "RCRH.Empleados.SP_ObtenerTurnoPlanillas",1)},
                  { 11, Tuple.Create("RCRH.Empleados.SP_ObtenerPlanillasContratos",1)},
                  { 12, Tuple.Create("RCCONFIG.Empresa.SP_ObtenerMonedaPago",2)},
                  { 13,Tuple.Create( "RCRH.Empleados.SP_ObtenerBancosPago",1)},
                  { 14, Tuple.Create("RCRH.Empleados.SP_ObtenerEmpleadosMovimientos",1)},
                  { 15,Tuple.Create( "RCRH.Empleados.SP_ObtenerEmpleadoDatosLaborales",3)},
                  { 16,Tuple.Create( "RCCONFIG.Empresa.SP_ObtenerEmpresas",2)},
                  { 17, Tuple.Create("RCRH.Empleados.SP_ObtenerCobrosControl",3)},
                  { 18, Tuple.Create("RCRH.Empleados.SP_ObtenerEmpleadoPlanillas",1)},
                  { 19, Tuple.Create("RCRH.Empleados.SP_RPT_PLANILLAHORIZONTAL",3)},
                  { 20, Tuple.Create("RCRH.Empleados.SP_VacacionesObtener",3)},
                  { 21, Tuple.Create("RCRH.Empleados.SP_ObtenerListaDocumentos",3)},
                  { 22, Tuple.Create("RCRH.Empleados.SP_DocumentosEmpleados_GetDel",3)}
                };

            if (!procedimientos.TryGetValue(OpcionSP, out var storedProcedure))
            {
                Console.WriteLine("Opción inválida");
            }

            using (var conexion = Conexion.AbrirConexion(storedProcedure.Item2))
            using (var comando = new SqlCommand(storedProcedure.Item1, conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Opcion", OpcionCampo);
                comando.Parameters.AddWithValue("@valor", valor);

                using (var lector = comando.ExecuteReader())
                {
                    tabla.Load(lector);
                }
            }
            return tabla;
        }
        public DataTable SP_MenuDinamico_GET(dynamic a)
        {
            leer = null;
            comando.Parameters.Clear();
            DataTable tabla = new DataTable();
            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }
            tabla = new DataTable();

            comando.Connection = Conexion.AbrirConexion(3);
            comando.Parameters.AddWithValue("@Opcion", a.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Valor", a.Valor ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Valor2", a.Valor2 ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Valor3", a.Valor3 ?? (object)DBNull.Value);

            comando.CommandText = "RCConfig.Empresa.SP_MenuDinamico_GET";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);
            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_ClientesINTER(dynamic cliente)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(5);

            comando.Parameters.AddWithValue("@Opcion", cliente.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Cliente", cliente.Cliente ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Nombre", cliente.Nombre ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@GrupoCliente", cliente.GrupoCliente ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@TipoPago", cliente.TipoPago ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@CreditoDia", cliente.CreditoDia ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@CreditoLimite", cliente.CreditoLimite ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@TipoCosto", cliente.TipoCosto ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Descuento", cliente.Descuento ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FechaIngreso", cliente.FechaIngreso ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@SolicitaGuia", cliente.SolicitaGuia ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PagoDestinatario", cliente.PagoDestinatario ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Bloqueado", cliente.Bloqueado ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@SucursalPago", cliente.SucursalPago ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Lunes", cliente.Lunes ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Martes", cliente.Martes ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Miercoles", cliente.Miercoles ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Jueves", cliente.Jueves ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Viernes", cliente.Viernes ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Sabados", cliente.Sabados ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Domingos", cliente.Domingos ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ConfirmarRecoleccion", cliente.ConfirmarRecoleccion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Empresa", cliente.Empresa ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", cliente.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", cliente.FPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", cliente.PC ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@RtnCedula", cliente.RtnCedula ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Inter", cliente.Inter ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Act", cliente.Act ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Pago", cliente.Pago ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PrimerNombre", cliente.PrimerNombre ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@SegundoNombre", cliente.SegundoNombre ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PrimerApellido", cliente.PrimerApellido ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@SegundoApellido", cliente.SegundoApellido ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FechaNacimiento", cliente.FechaNacimiento ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@EnviosRecientes", cliente.EnviosRecientes ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Aereo", cliente.Aereo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Password", cliente.Password ?? (object)DBNull.Value);

            comando.CommandText = "ENAC.dbo.SP_Clientes";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }

        public DataTable SP_ClientesENAC(dynamic cliente)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(4);

            comando.Parameters.AddWithValue("@Opcion", cliente.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Cliente", cliente.Cliente ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Nombre", cliente.Nombre ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@GrupoCliente", cliente.GrupoCliente ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@TipoPago", cliente.TipoPago ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@CreditoDia", cliente.CreditoDia ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@CreditoLimite", cliente.CreditoLimite ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@TipoCosto", cliente.TipoCosto ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Descuento", cliente.Descuento ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FechaIngreso", cliente.FechaIngreso ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@SolicitaGuia", cliente.SolicitaGuia ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PagoDestinatario", cliente.PagoDestinatario ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Bloqueado", cliente.Bloqueado ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@SucursalPago", cliente.SucursalPago ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Lunes", cliente.Lunes ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Martes", cliente.Martes ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Miercoles", cliente.Miercoles ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Jueves", cliente.Jueves ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Viernes", cliente.Viernes ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Sabados", cliente.Sabados ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Domingos", cliente.Domingos ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ConfirmarRecoleccion", cliente.ConfirmarRecoleccion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Empresa", cliente.Empresa ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", cliente.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", cliente.FPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", cliente.PC ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@RtnCedula", cliente.RtnCedula ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Inter", cliente.Inter ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Act", cliente.Act ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Semanales", cliente.Semanales ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Mensuales", cliente.Mensuales ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Quincenales", cliente.Quincenales ?? (object)DBNull.Value);

            comando.CommandText = "ENAC.dbo.SP_Clientes";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }


        public DataTable SP_Clientes(dynamic cliente)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(3);

            comando.Parameters.AddWithValue("@Opcion", cliente.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@EstadoENAC", cliente.EstadoENAC ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ClienteID", cliente.ClienteID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@NombreCompleto", cliente.NombreCompleto ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@GrupoClienteID", cliente.GrupoClienteID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@CondicionPagoID", cliente.CondicionPagoID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FechaRegistro", cliente.FechaRegistro ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@SucursalID", cliente.SucursalID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@EmpresaID", cliente.EmpresaID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@TipoClienteID", cliente.TipoClienteID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@DiasCredito", cliente.DiasCredito ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@LimiteCredito", cliente.LimiteCredito ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@TipoDocumentoID", cliente.TipoDocumentoID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@DocumentoValor", cliente.DocumentoValor ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", cliente.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", cliente.FPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", cliente.PC ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Estado", cliente.Estado ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@inter", cliente.inter ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@nac", cliente.nac ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Aereo", cliente.Aereo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Imagen", cliente.Imagen ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Telefono", cliente.Telefono ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Correo", cliente.Correo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@TipoCosto", cliente.TipoCosto ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Descuento", cliente.Descuento ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FechaNacimiento", cliente.FechaNacimiento ?? (object)DBNull.Value);

            comando.CommandText = "RC.Clientes.SP_Clientes";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_PermisosEspecificos(dynamic permiso)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            // Limpiar la tabla si ya tiene datos
            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(3);

            // Agregar parámetros
            comando.Parameters.AddWithValue("@Opcion", permiso.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PermisoID", permiso.PermisoID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UsuarioID", permiso.UsuarioID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@NombreFormulario", permiso.NombreFormulario ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@NombreElemento", permiso.NombreElemento ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@AccionElemento", permiso.AccionElemento ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@IconoElemento", permiso.IconoElemento ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Visible", permiso.Visible ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UbicacionID", permiso.UbicacionID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", permiso.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", permiso.FPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", permiso.PC ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Estado", permiso.Estado ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ModuloID", permiso.ModuloID ?? (object)DBNull.Value);

            // Configurar comando
            comando.CommandText = "RCCONFIG.Empresa.SP_PermisosEspecificos";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_EstadoENAC(dynamic activar)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            // Limpiar la tabla si ya tiene datos
            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(3);

            // Agregar parámetros
            comando.Parameters.AddWithValue("@Opcion", activar.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ID", activar.ID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Descripcion", activar.Descripcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Estado", activar.Estado ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", activar.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", activar.FPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", activar.PC ?? (object)DBNull.Value);

            // Configurar comando
            comando.CommandText = "RCCONFIG.Empresa.SP_ActivarENAC";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_GrupoClientes(dynamic grupo)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            // Limpiar la tabla si ya tiene datos
            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(3);

            comando.Parameters.AddWithValue("@Opcion", grupo.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@GrupoClientesID", grupo.GrupoClientesID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Nombre", grupo.Nombre ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@EmpresaID", grupo.EmpresaID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", grupo.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", grupo.FPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", grupo.PC ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Estado", grupo.Estado ?? (object)DBNull.Value);

            comando.CommandText = "RC.Clientes.SP_GrupoClientes";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_TBLClientesTipoPago(dynamic a)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            // Limpiar la tabla si ya tiene datos
            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(3);

            comando.Parameters.AddWithValue("@Opcion", a.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Valor", a.Valor ?? (object)DBNull.Value);

            comando.CommandText = "RC.Clientes.SP_TBLClientesTipoPago_GET";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_CondicionPago(dynamic a)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            comando.Connection = Conexion.AbrirConexion(3); // Ajustá el índice de conexión si aplica
            comando.CommandText = "RC.Clientes.SP_CondicionPago";
            comando.CommandType = CommandType.StoredProcedure;

            // Parámetros del procedimiento
            comando.Parameters.AddWithValue("@Opcion", a.Opcion);
            comando.Parameters.AddWithValue("@CondicionPagoID", (object)a.CondicionPagoID ?? DBNull.Value);
            comando.Parameters.AddWithValue("@Descripcion", (object)a.Descripcion ?? DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", (object)a.UPosteo ?? DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", (object)a.FPosteo ?? DBNull.Value);
            comando.Parameters.AddWithValue("@PC", (object)a.PC ?? DBNull.Value);
            comando.Parameters.AddWithValue("@Estado", (object)a.Estado ?? DBNull.Value);

            leer = comando.ExecuteReader();
            tabla.Load(leer);
            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_ClientesDocumentos(dynamic documento)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(3);

            comando.Parameters.AddWithValue("@Opcion", documento.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@DocumentoID", documento.DocumentoID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Descripcion", documento.Descripcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Estado", documento.Estado ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", documento.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", documento.FPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", documento.PC ?? (object)DBNull.Value);

            comando.CommandText = "RC.Clientes.SP_ClientesDocumentos";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_ObtenerCiudades(dynamic a)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            // Limpiar la tabla si ya tiene datos
            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(3);

            comando.Parameters.AddWithValue("@Opcion", a.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@valor", a.valor ?? (object)DBNull.Value);

            comando.CommandText = "RCCONFIG.Empresa.SP_ObtenerCiudades";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_ObtenerPaises(dynamic a)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            // Limpiar la tabla si ya tiene datos
            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(3);

            comando.Parameters.AddWithValue("@Opcion", a.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@valor", a.valor ?? (object)DBNull.Value);

            comando.CommandText = "RCCONFIG.Empresa.SP_ObtenerPaises";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_SeguimientoUsuario(dynamic seguimiento)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(3);

            comando.Parameters.AddWithValue("@Operacion", seguimiento.Operacion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@RegistroID", seguimiento.RegistroID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Usuario", seguimiento.Usuario ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Modulo", seguimiento.Modulo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Formulario", seguimiento.Formulario ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@AccionID", seguimiento.AccionID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", seguimiento.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", seguimiento.FPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", seguimiento.PC ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Estado", seguimiento.Estado ?? (object)DBNull.Value);

            comando.CommandText = "RCHISTORIAL.Registros.SP_AdministrarSeguimientoUsuario";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_AdministrarDiasCredito(dynamic credito)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(3);

            comando.Parameters.AddWithValue("@Opcion", credito.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@DiasCreditoID", credito.DiasCreditoID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Descripcion", credito.Descripcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Estado", credito.Estado ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", credito.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", credito.FPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", credito.PC ?? (object)DBNull.Value);

            comando.CommandText = "RC.Clientes.SP_AdministrarDiasCredito";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_ClienteCiudades(dynamic ciudad)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(3);

            comando.Parameters.AddWithValue("@Opcion", ciudad.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ClienteCiudadesID", ciudad.ClienteCiudadesID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ClienteID", ciudad.ClienteID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@CiudadID", ciudad.CiudadID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Direccion", ciudad.Direccion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@TelefonoPrincipal", ciudad.TelefonoPrincipal ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ContactoNombre", ciudad.ContactoNombre ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ContactoTelefono", ciudad.ContactoTelefono ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", ciudad.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", ciudad.FPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", ciudad.PC ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Estado", ciudad.Estado ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ClienteCiudadIDAnterior", ciudad.ClienteCiudadIDAnterior ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PaisID", ciudad.PaisID ?? (object)DBNull.Value);

            comando.CommandText = "RC.Clientes.SP_ClienteCiudades";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_ClienteCiudadesENAC(dynamic ciudad)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(4);

            comando.Parameters.AddWithValue("@Opcion", ciudad.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Cliente", ciudad.Cliente ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Ciudad", ciudad.Ciudad ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Direccion", ciudad.Direccion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Inter", ciudad.Inter ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Colonia", ciudad.Colonia ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Telefono", ciudad.Telefono ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Fax", ciudad.Fax ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Correo", ciudad.Correo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ContactoNombre", ciudad.ContactoNombre ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ContactoCorreo", ciudad.ContactoCorreo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ContactoTelefono", ciudad.ContactoTelefono ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ContactoDepto", ciudad.ContactoDepto ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Empresa", ciudad.Empresa ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", ciudad.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", ciudad.FPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", ciudad.PC ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Act", ciudad.Act ?? (object)DBNull.Value);

            comando.CommandText = "ENAC.dbo.SP_ClienteCiudades";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_ClienteCiudadesINTER(dynamic ciudad)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(5);

            comando.Parameters.AddWithValue("@Opcion", ciudad.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Cliente", ciudad.Cliente ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Ciudad", ciudad.Ciudad ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Direccion", ciudad.Direccion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Inter", ciudad.Inter ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Colonia", ciudad.Colonia ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Telefono", ciudad.Telefono ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Fax", ciudad.Fax ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Correo", ciudad.Correo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ContactoNombre", ciudad.ContactoNombre ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ContactoCorreo", ciudad.ContactoCorreo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ContactoTelefono", ciudad.ContactoTelefono ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ContactoDepto", ciudad.ContactoDepto ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Empresa", ciudad.Empresa ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", ciudad.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", ciudad.FPosteo != DateTime.MinValue ? ciudad.FPosteo : (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", ciudad.PC ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Act", ciudad.Act ?? (object)DBNull.Value);

            comando.CommandText = "ENAC.dbo.SP_ClienteCiudades";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_ClientesCostos(dynamic cliente)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(3);

            comando.Parameters.AddWithValue("@Opcion", cliente.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ClienteCostosID", cliente.ClienteCostosID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Descripcion", cliente.Descripcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", cliente.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", cliente.FPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", cliente.PC ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Estado", cliente.Estado ?? (object)DBNull.Value);

            comando.CommandText = "RC.Clientes.SP_ClientesCostos";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_BodegasContadoresENAC(dynamic bodega)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(4);

            comando.Parameters.AddWithValue("@Opcion", bodega.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Bodega", bodega.Bodega ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Nombre", bodega.Nombre ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Caracter", bodega.Caracter ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Contador", bodega.Contador ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Empresa", bodega.Empresa ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", bodega.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Fposteo", bodega.Fposteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", bodega.PC ?? (object)DBNull.Value);

            comando.CommandText = "ENAC.dbo.SP_Bodegas";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_BodegasContadoresINTER(dynamic bodega)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(5);

            comando.Parameters.AddWithValue("@Opcion", bodega.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Bodega", bodega.Bodega ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Nombre", bodega.Nombre ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Caracter", bodega.Caracter ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Contador", bodega.Contador ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Empresa", bodega.Empresa ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", bodega.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Fposteo", bodega.Fposteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", bodega.PC ?? (object)DBNull.Value);

            comando.CommandText = "ENAC.dbo.SP_Bodegas";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_FacturasENAC(dynamic factura)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(4);

            comando.Parameters.AddWithValue("@Opcion", factura.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Factura", factura.Factura ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ClienteRemitente", factura.ClienteRemitente ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ClienteDestino", factura.ClienteDestino ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Guia", factura.Guia ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Empresa", factura.Empresa ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FechaInicio", factura.FechaInicio ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FechaFin", factura.FechaFin ?? (object)DBNull.Value);

            comando.CommandText = "ENAC.dbo.SP_Facturas";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_FacturasProceso(dynamic proceso)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(4); // Ajusta el índice según tu lógica de conexión

            comando.Parameters.AddWithValue("@Opcion", proceso.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ProcesoID", proceso.ProcesoID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FacturaID", proceso.FacturaID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Proceso", proceso.Proceso ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", proceso.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", proceso.FPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", proceso.PC ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Guia", proceso.Guia ?? (object)DBNull.Value);

            comando.CommandText = "ENAC.dbo.SP_FacturasProceso";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_MetodoPagos(dynamic metodo)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(3); 

            comando.Parameters.AddWithValue("@Opcion", metodo.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@MetodoID", metodo.MetodoID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Descripcion", metodo.Descripcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Estado", metodo.Estado ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", metodo.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", metodo.FPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", metodo.PC ?? (object)DBNull.Value);

            comando.CommandText = "RCCONFIG.Empresa.SP_MetodoPagos";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_CobroCajaEncabezado(dynamic cobroEnc)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(3);

            comando.Parameters.AddWithValue("@Opcion", cobroEnc.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ID", cobroEnc.ID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ControlCajaID", cobroEnc.ControlCajaID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FacturaID", cobroEnc.FacturaID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@TotalAPagar", cobroEnc.TotalAPagar ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@TotalRecibido", cobroEnc.TotalRecibido ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@TotalCambio", cobroEnc.TotalCambio ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", cobroEnc.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", cobroEnc.FPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", cobroEnc.PC ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Estado", cobroEnc.Estado ?? (object)DBNull.Value);

            comando.CommandText = "RCCONFIG.Facturacion.SP_CobroCajaEncabezado";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_CobroCajaMetodos(dynamic cobroMetodo)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(3);

            comando.Parameters.AddWithValue("@Opcion", cobroMetodo.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ID", cobroMetodo.ID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@EncabezadoID", cobroMetodo.EncabezadoID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@MetodoPagoID", cobroMetodo.MetodoPagoID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@MontoIngresado", cobroMetodo.MontoIngresado ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Referencia", cobroMetodo.Referencia ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", cobroMetodo.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", cobroMetodo.FPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", cobroMetodo.PC ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Estado", cobroMetodo.Estado ?? (object)DBNull.Value);

            comando.CommandText = "RCCONFIG.Facturacion.SP_CobroCajaMetodos";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }
        public DataTable SP_ControlCaja(dynamic controlCaja)
        {
            SqlDataReader leer = null;
            SqlCommand comando = new SqlCommand();
            DataTable tabla = new DataTable();

            if (tabla.Rows.Count > 0)
            {
                tabla.Rows.Clear();
                tabla.Clear();
            }

            comando.Connection = Conexion.AbrirConexion(3);

            comando.Parameters.AddWithValue("@Opcion", controlCaja.Opcion ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@ControlID", controlCaja.ControlID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@TipoID", controlCaja.TipoID ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@MontoCheque", controlCaja.MontoCheque ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@MontoEfectivo", controlCaja.MontoEfectivo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@MontoTarjeta", controlCaja.MontoTarjeta ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@MontoTransferencia", controlCaja.MontoTransferencia ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@MontoTotal", controlCaja.MontoTotal ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@VarianzaCheque", controlCaja.VarianzaCheque ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@VarianzaEfectivo", controlCaja.VarianzaEfectivo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@VarianzaTarjeta", controlCaja.VarianzaTarjeta ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@VarianzaTransferencia", controlCaja.VarianzaTransferencia ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@UPosteo", controlCaja.UPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FPosteo", controlCaja.FPosteo ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@PC", controlCaja.PC ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Estado", controlCaja.Estado ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FechaInicio", controlCaja.FechaInicio ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@FechaFinal", controlCaja.FechaFinal ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@Comentario", controlCaja.Comentario ?? (object)DBNull.Value);

            comando.CommandText = "RCCONFIG.Facturacion.SP_ControlCaja";
            comando.CommandType = CommandType.StoredProcedure;

            leer = comando.ExecuteReader();
            tabla.Load(leer);

            Conexion.CerrarConexion();

            return tabla;
        }

    }
}
