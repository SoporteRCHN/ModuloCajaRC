using Datos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica
{
    public class clsLogica
    {
        private readonly EnviarConsultas enviar = new EnviarConsultas();
        public DataTable SP_MenuDinamico_GET(TBLMenuDinamicoLista a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_MenuDinamico_GET(a);
            return tabla;
        }
        public DataTable SP_Clientes(Cliente a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_Clientes(a);
            return tabla;
        }
        public DataTable SP_ClientesENAC(ClienteENAC a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_ClientesENAC(a);
            return tabla;
        }
        public DataTable SP_ClientesINTER(ClienteINTER a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_ClientesINTER(a);
            return tabla;
        }
        public DataTable SP_ClienteCiudades(ClienteCiudad a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_ClienteCiudades(a);
            return tabla;
        }
        public DataTable SP_ClienteCiudadesENAC(ClienteCiudadENAC a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_ClienteCiudadesENAC(a);
            return tabla;
        }
        public DataTable SP_ClienteCiudadesINTER(ClienteCiudadINTER a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_ClienteCiudadesINTER(a);
            return tabla;
        }
        public DataTable SP_PermisosEspecificos(TBLPermisosEspecificos a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_PermisosEspecificos(a);
            return tabla;
        }
        public DataTable SP_EstadoENAC(EstadoENAC a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_EstadoENAC(a);
            return tabla;
        }
        public DataTable SP_GrupoClientes(TBLGrupoClientes a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_GrupoClientes(a);
            return tabla;
        }
        public DataTable SP_TBLClientesTipoPago(TipoPago a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_TBLClientesTipoPago(a);
            return tabla;
        }
        public DataTable SP_CondicionPago(CondicionPago a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_CondicionPago(a);
            return tabla;
        }
        public DataTable SP_ClientesDocumentos(DocumentoCliente a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_ClientesDocumentos(a);
            return tabla;
        }
        public DataTable SP_ObtenerCiudades(CiudadesClientes a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_ObtenerCiudades(a);
            return tabla;
        }
        public DataTable SP_ObtenerPaises(PaisesClientes a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_ObtenerPaises(a);
            return tabla;
        }
        public DataTable SP_SeguimientoUsuario(SeguimientoUsuario a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_SeguimientoUsuario(a);
            return tabla;
        }
        public DataTable SP_AdministrarDiasCredito(DiasCreditoCliente a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_AdministrarDiasCredito(a);
            return tabla;
        }
        public DataTable SP_ClientesCostos(ClienteCosto a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_ClientesCostos(a);
            return tabla;
        }
        public DataTable SP_BodegasContadoresENAC(BodegaDTO a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_BodegasContadoresENAC(a);
            return tabla;
        }
        public DataTable SP_BodegasContadoresINTER(BodegaDTO a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_BodegasContadoresINTER(a);
            return tabla;
        }
        public DataTable SP_FacturasENAC(FacturaDTO a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_FacturasENAC(a);
            return tabla;
        }
        public DataTable SP_FacturasProceso(FacturaProcesoDTO a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_FacturasProceso(a);
            return tabla;
        }
        public DataTable SP_FacturasProceso50(FacturaProcesoDTO50 a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_FacturasProceso50(a);
            return tabla;
        }
        public DataTable SP_MetodoPagos(MetodoPagoDTO a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_MetodoPagos(a);
            return tabla;
        }
        public DataTable SP_CobroCajaEncabezado(CobroCajaEncabezadoDTO a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_CobroCajaEncabezado(a);
            return tabla;
        }
        public DataTable SP_CobroCajaMetodos(CobroCajaMetodosDTO a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_CobroCajaMetodos(a);
            return tabla;
        }
        public DataTable SP_ControlCaja(ControlCajaDTO a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_ControlCaja(a);
            return tabla;
        }
        public DataTable SP_FactorDolar50(FactorDolar50DTO a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_FactorDolar50(a);
            return tabla;
        }
        public DataTable SP_FactorDolarHistorico50(FactorDolarHistorico50DTO a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_FactorDolarHistorico50(a);
            return tabla;
        }
        public DataTable SP_Facturas250(Factura2DTO50 a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_Facturas250(a);
            return tabla;
        }
        public DataTable SP_SucursalUbicaciones(SucursalUbicacionDTO a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_SucursalUbicaciones(a);
            return tabla;
        }
        public DataTable SP_PlanContingencia(PlanContingenciaDTO a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_PlanContingencia(a);
            return tabla;
        }
        public DataTable SP_FacturasProcesoActual(FacturaProcesoActualDTO a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_FacturasProcesoActual(a);
            return tabla;
        }
        public DataTable SP_TBLUsuarios(TBLUsuariosDTO a)
        {
            DataTable tabla = new DataTable();
            tabla = enviar.SP_TBLUsuarios(a);
            return tabla;
        }
    }
}
