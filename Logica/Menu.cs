using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica
{
    public class Menu
    {
    }
    public class TBLMenuDinamicoLista
    {
        public string Opcion { get; set; }
        public string Valor { get; set; }
        public string Valor2 { get; set; }
        public string Valor3 { get; set; }
    }
    public class PermisoAprobacionListado
    {
        public string Opcion { get; set; }
        public int? PermisoID { get; set; }
        public string Usuario { get; set; }
        public string NombreFormulario { get; set; }
        public string TipoObjeto { get; set; }
        public string NombreObjeto { get; set; }
        public string ValorObjeto { get; set; }
        public int? FormaValor { get; set; }
        public int? Estado { get; set; }
        public DateTime? FPosteo { get; set; }
        public string UPosteo { get; set; }
    }
    public class ClienteINTER
    {
        public string Opcion { get; set; }
        public string Cliente { get; set; }
        public string Nombre { get; set; }
        public string GrupoCliente { get; set; }
        public short? TipoPago { get; set; }
        public short? CreditoDia { get; set; }
        public decimal? CreditoLimite { get; set; }
        public short? TipoCosto { get; set; }
        public decimal? Descuento { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public bool? SolicitaGuia { get; set; }
        public bool? PagoDestinatario { get; set; }
        public bool? Bloqueado { get; set; }
        public string SucursalPago { get; set; }
        public bool? Lunes { get; set; }
        public bool? Martes { get; set; }
        public bool? Miercoles { get; set; }
        public bool? Jueves { get; set; }
        public bool? Viernes { get; set; }
        public bool? Sabados { get; set; }
        public bool? Domingos { get; set; }
        public bool? ConfirmarRecoleccion { get; set; }
        public string Empresa { get; set; }
        public string UPosteo { get; set; }
        public DateTime? FPosteo { get; set; }
        public string PC { get; set; }
        public string RtnCedula { get; set; }
        public bool? Inter { get; set; }
        public string Act { get; set; }
        public string Pago { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string EnviosRecientes { get; set; }
        public string Aereo { get; set; }
        public string Password { get; set; }
    }

    public class ClienteENAC
    {
        public string Opcion { get; set; }
        public string Cliente { get; set; }
        public string Nombre { get; set; }
        public string GrupoCliente { get; set; }
        public short TipoPago { get; set; }
        public short CreditoDia { get; set; }
        public decimal CreditoLimite { get; set; }
        public short TipoCosto { get; set; }
        public decimal Descuento { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public bool? SolicitaGuia { get; set; }
        public bool PagoDestinatario { get; set; }
        public bool Bloqueado { get; set; }
        public string SucursalPago { get; set; }
        public bool Lunes { get; set; }
        public bool Martes { get; set; }
        public bool Miercoles { get; set; }
        public bool Jueves { get; set; }
        public bool Viernes { get; set; }
        public bool Sabados { get; set; }
        public bool Domingos { get; set; }
        public bool ConfirmarRecoleccion { get; set; }
        public string Empresa { get; set; }
        public string UPosteo { get; set; }
        public DateTime? FPosteo { get; set; }
        public string PC { get; set; }
        public string RtnCedula { get; set; }
        public bool? Inter { get; set; }
        public string Act { get; set; }
        public string Semanales { get; set; }
        public string Mensuales { get; set; }
        public string Quincenales { get; set; }
    }

    public class Cliente
    {
        public string Opcion { get; set; }
        public bool EstadoENAC { get; set; }
        public int? ClienteID { get; set; }
        public string NombreCompleto { get; set; }
        public int? GrupoClienteID { get; set; }
        public int? CondicionPagoID { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? SucursalID { get; set; }
        public int? EmpresaID { get; set; }
        public int? TipoClienteID { get; set; }
        public int? DiasCredito { get; set; }
        public decimal? LimiteCredito { get; set; }
        public int? TipoDocumentoID { get; set; }
        public string DocumentoValor { get; set; }
        public string UPosteo { get; set; }
        public DateTime? FPosteo { get; set; }
        public string PC { get; set; }
        public bool? Estado { get; set; }
        public int? inter { get; set; }
        public int? nac { get; set; }
        public int? Aereo { get; set; }
        public string Imagen { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public int? TipoCosto { get; set; }
        public decimal? Descuento { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        
    }

    public class TBLPermisosEspecificos
    {
        public string Opcion { get; set; }               // Acción a ejecutar en el SP
        public int? PermisoID { get; set; }              // Clave primaria
        public int? UsuarioID { get; set; }              // Usuario al que se le asigna el permiso
        public string NombreFormulario { get; set; }     // Nombre del formulario
        public string NombreElemento { get; set; }       // Nombre del elemento
        public string AccionElemento { get; set; }       // Nombre visual para el usuario
        public string IconoElemento { get; set; }        // Ícono asociado (opcional)
        public bool? Visible { get; set; }               // Visibilidad del elemento
        public int? UbicacionID { get; set; }           // Ubicacion del elemento
        public string UPosteo { get; set; }              // Usuario que registra
        public DateTime? FPosteo { get; set; }           // Fecha de registro
        public string PC { get; set; }                   // Nombre del equipo
        public bool? Estado { get; set; }                // Estado activo/inactivo
        public int? ModuloID { get; set; }
    }
    public class EstadoENAC
    {
        public string Opcion { get; set; }           // Acción a ejecutar en el SP
        public int? ID { get; set; }                 // Clave primaria
        public string Descripcion { get; set; }      // Descripción del registro
        public bool? Estado { get; set; }            // Estado activo/inactivo
        public string UPosteo { get; set; }          // Usuario que registra
        public DateTime? FPosteo { get; set; }       // Fecha de registro
        public string PC { get; set; }               // Nombre del equipo
    }
    public class TBLGrupoClientes
    {
        public string Opcion { get; set; }           // Acción a ejecutar en el SP
        public int? GrupoClientesID { get; set; }    // Clave primaria
        public string Nombre { get; set; }           // Nombre del grupo
        public int? EmpresaID { get; set; }          // Empresa asociada
        public bool? Estado { get; set; }            // Estado activo/inactivo
        public string UPosteo { get; set; }          // Usuario que registra
        public DateTime? FPosteo { get; set; }       // Fecha de registro
        public string PC { get; set; }               // Nombre del equipo
    }
    public class TipoPago
    {
        public int Opcion { get; set; }           // Acción a ejecutar en el SP
        public string Valor { get; set; }           // Tipo de PAgo
    }
    public class CondicionPago
    {
        public string Opcion { get; set; }             // Acción a ejecutar en el SP
        public int? CondicionPagoID { get; set; }      // ID autogenerado
        public string Descripcion { get; set; }        // Nombre de la condición de pago
        public string UPosteo { get; set; }            // Usuario que realiza el cambio
        public DateTime? FPosteo { get; set; }         // Fecha del cambio
        public string PC { get; set; }                 // PC desde donde se hizo el cambio
        public bool? Estado { get; set; }              // true = activo, false = inactivo
    }
    public class DocumentoCliente
    {
        public string Opcion { get; set; }             // Acción a ejecutar en el SP
        public int? DocumentoID { get; set; }          // ID autogenerado del documento
        public string Descripcion { get; set; }        // Nombre o tipo del documento
        public bool? Estado { get; set; }              // true = activo, false = inactivo
        public string UPosteo { get; set; }            // Usuario que realiza el cambio
        public DateTime? FPosteo { get; set; }         // Fecha del cambio
        public string PC { get; set; }                 // PC desde donde se hizo el cambio
    }
    public class CiudadesClientes
    {
        public int Opcion { get; set; }           // Acción a ejecutar en el SP
        public string valor { get; set; }
    }
    public class PaisesClientes
    {
        public int Opcion { get; set; }           // Acción a ejecutar en el SP
        public string valor { get; set; }           // Tipo de PAgo
    }
    public class SeguimientoUsuario
    {
        public string Operacion { get; set; }             // Acción a ejecutar en el SP: INSERTAR, ACTUALIZAR, INACTIVAR, LISTAR
        public int? RegistroID { get; set; }              // ID autogenerado del registro
        public string Usuario { get; set; }               // Usuario que realizó la acción
        public string Modulo { get; set; }                // Módulo del sistema donde ocurrió
        public string Formulario { get; set; }            // Formulario específico
        public int? AccionID { get; set; }                // ID de la acción realizada
        public string UPosteo { get; set; }               // Usuario que realiza el cambio
        public DateTime? FPosteo { get; set; }            // Fecha del cambio
        public string PC { get; set; }                    // PC desde donde se hizo el cambio
        public bool? Estado { get; set; }                 // true = activo, false = inactivo
    }
    public class DiasCreditoCliente
    {
        public string Opcion { get; set; }           // Acción a ejecutar en el SP: INSERTAR, ACTUALIZAR, INACTIVAR, LISTAR
        public int? DiasCreditoID { get; set; }         // ID autogenerado del registro
        public string Descripcion { get; set; }         // Descripción del tipo de crédito (ej. "30 días", "Contado")
        public bool? Estado { get; set; }               // true = activo, false = inactivo
        public string UPosteo { get; set; }             // Usuario que realiza el cambio
        public DateTime? FPosteo { get; set; }          // Fecha del cambio
        public string PC { get; set; }                  // PC desde donde se hizo el cambio
    }
    public class ClienteCiudad
    {
        public string Opcion { get; set; }
        public int? ClienteCiudadesID { get; set; }
        public int? ClienteID { get; set; }
        public int? CiudadID { get; set; }
        public string Direccion { get; set; }
        public string TelefonoPrincipal { get; set; }
        public string ContactoNombre { get; set; }
        public string ContactoTelefono { get; set; }
        public string UPosteo { get; set; }
        public DateTime? FPosteo { get; set; }
        public string PC { get; set; }
        public bool? Estado { get; set; }
        public string ClienteCiudadIDAnterior { get; set; }
        public int? PaisID { get; set; }
    }
    public class ClienteCiudadENAC
    {
        public string Opcion { get; set; } // Para operaciones tipo 'AGREGAR', 'ACTUALIZAR', etc.
        public string Cliente { get; set; }
        public string Ciudad { get; set; }
        public string Direccion { get; set; }
        public string Inter { get; set; }
        public string Colonia { get; set; }
        public string Telefono { get; set; }
        public string Fax { get; set; }
        public string Correo { get; set; }
        public string ContactoNombre { get; set; }
        public string ContactoCorreo { get; set; }
        public string ContactoTelefono { get; set; }
        public string ContactoDepto { get; set; }
        public string Empresa { get; set; }
        public string UPosteo { get; set; }
        public DateTime? FPosteo { get; set; }
        public string PC { get; set; }
        public string Act { get; set; }
    }
    public class ClienteCiudadINTER
    {
        public string Opcion { get; set; }
        public string Cliente { get; set; }
        public string Ciudad { get; set; }
        public string Direccion { get; set; }
        public string Inter { get; set; }
        public string Colonia { get; set; }
        public string Telefono { get; set; }
        public string Fax { get; set; }
        public string Correo { get; set; }
        public string ContactoNombre { get; set; }
        public string ContactoCorreo { get; set; }
        public string ContactoTelefono { get; set; }
        public string ContactoDepto { get; set; }
        public string Empresa { get; set; }
        public string UPosteo { get; set; }
        public DateTime FPosteo { get; set; }
        public string PC { get; set; }
        public string Act { get; set; }
    }
    public class ClienteCosto
    {
        public string Opcion { get; set; }
        public int? ClienteCostosID { get; set; }
        public string Descripcion { get; set; }
        public string UPosteo { get; set; }
        public DateTime? FPosteo { get; set; }
        public string PC { get; set; }
        public bool? Estado { get; set; }
    }
    public class BodegaDTO
    {
        public string Opcion { get; set; }
        public string Bodega { get; set; }
        public string Nombre { get; set; }
        public string Caracter { get; set; }
        public long? Contador { get; set; }
        public string Empresa { get; set; }
        public string UPosteo { get; set; }
        public DateTime? Fposteo { get; set; }
        public string PC { get; set; }
    }
    public class FacturaDTO
    {
        public string Opcion { get; set; }
        public string Factura { get; set; }
        public string ClienteRemitente { get; set; }
        public string ClienteDestino { get; set; }
        public string Guia { get; set; }
        public string Empresa { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
    public class FacturaProcesoDTO
    {
        public string Opcion { get; set; }             // Acción a ejecutar en el SP (AGREGAR, LISTADO, etc.)
        public long? ProcesoID { get; set; }           // Clave primaria del proceso
        public int? FacturaID { get; set; }            // ID de la factura asociada
        public int? Proceso { get; set; }              // Estado o tipo de proceso
        public string UPosteo { get; set; }            // Usuario que registra el proceso
        public DateTime? FPosteo { get; set; }         // Fecha del registro
        public string PC { get; set; }                 // Nombre del equipo desde donde se registró
        public string Guia { get; set; }
    }
    public class MetodoPagoDTO
    {
        public string Opcion { get; set; }           // Acción a ejecutar en el SP (AGREGAR, LISTADO, etc.)
        public int? MetodoID { get; set; }           // Clave primaria del método de pago
        public string Descripcion { get; set; }      // Nombre o descripción del método
        public bool? Estado { get; set; }            // Estado activo/inactivo
        public string UPosteo { get; set; }          // Usuario que registra el método
        public DateTime? FPosteo { get; set; }       // Fecha de registro
        public string PC { get; set; }               // Nombre del equipo desde donde se registró
    }
    public class CobroCajaEncabezadoDTO
    {
        public string Opcion { get; set; }               // Acción a ejecutar en el SP (AGREGAR, LISTADO, etc.)
        public long? ID { get; set; }                    // Clave primaria del cobro
        public int? ControlCajaID { get; set; }              // ID del lote en uso en la caja
        public int? FacturaID { get; set; }              // ID de la factura asociada
        public decimal? TotalAPagar { get; set; }        // Monto total a pagar
        public decimal? TotalRecibido { get; set; }      // Monto recibido por el cliente
        public decimal? TotalCambio { get; set; }        // Monto de cambio entregado
        public string UPosteo { get; set; }              // Usuario que registra el cobro
        public DateTime? FPosteo { get; set; }           // Fecha de registro
        public string PC { get; set; }                   // Nombre del equipo desde donde se registró
        public bool? Estado { get; set; }                // Estado activo/inactivo
        public string Origen { get; set; }
    }
    public class CobroCajaMetodosDTO
    {
        public string Opcion { get; set; }               // Acción a ejecutar en el SP
        public long? ID { get; set; }                    // Clave primaria
        public long? EncabezadoID { get; set; }          // ID del encabezado de cobro
        public int? MetodoPagoID { get; set; }           // ID del método de pago
        public decimal? MontoIngresado { get; set; }     // Monto ingresado
        public string Referencia { get; set; }           // Referencia del pago
        public string UPosteo { get; set; }              // Usuario que registra
        public DateTime? FPosteo { get; set; }           // Fecha de registro
        public string PC { get; set; }                   // Equipo desde donde se registró
        public bool? Estado { get; set; }                // Estado activo/inactivo
        public decimal? EfectivoRecibido { get; set; }
    }
    public class ControlCajaDTO
    {
        public string Opcion { get; set; }                  // Acción a ejecutar en el SP
        public long? ControlID { get; set; }                // Clave primaria del control
        public int? TipoID { get; set; }                    // Tipo de operación (apertura, cierre, etc.)
        public decimal? MontoCheque { get; set; }           // Monto en cheque
        public decimal? MontoEfectivo { get; set; }         // Monto en efectivo
        public decimal? MontoTarjeta { get; set; }          // Monto con tarjeta
        public decimal? MontoTransferencia { get; set; }    // Monto por transferencia
        public decimal? MontoTotal { get; set; }            // Monto total
        public decimal? VarianzaCheque { get; set; }           // Monto en cheque
        public decimal? VarianzaEfectivo { get; set; }         // Monto en efectivo
        public decimal? VarianzaTarjeta { get; set; }          // Monto con tarjeta
        public decimal? VarianzaTransferencia { get; set; }    // Monto por transferencia
        public string UPosteo { get; set; }                 // Usuario que registra
        public DateTime? FPosteo { get; set; }              // Fecha de registro
        public string PC { get; set; }                      // Equipo desde donde se registró
        public bool? Estado { get; set; }                   // Estado activo/inactivo
        public DateTime? FechaInicio { get; set; }          // FechaInicio
        public DateTime? FechaFinal { get; set; }           // FechaFinal
        public string Comentario { get; set; }                 //comentario de cierre
    }
    public class FacturaProcesoDTO50
    {
        public string Opcion { get; set; }             // Acción a ejecutar en el SP (AGREGAR, LISTADO, etc.)
        public long? ProcesoID { get; set; }           // Clave primaria del proceso
        public int? FacturaID { get; set; }            // ID de la factura asociada
        public int? Proceso { get; set; }              // Estado o tipo de proceso
        public string UPosteo { get; set; }            // Usuario que registra el proceso
        public DateTime? FPosteo { get; set; }         // Fecha del registro
        public string PC { get; set; }                 // Nombre del equipo desde donde se registró
        public string Guia { get; set; }
    }
    public class FactorDolar50DTO
    {
        public string Opcion { get; set; }            // Acción a ejecutar en el SP (AGREGAR, LISTADO, RECUPERAR, etc.)
        public decimal? FactorDolar { get; set; }     // Valor del factor de cambio (USD)
    }
    public class FactorDolarHistorico50DTO
    {
        public string Opcion { get; set; }            // Acción a ejecutar en el SP
        public long? FactorID { get; set; }           // ID del registro histórico
        public decimal? FactorDolar { get; set; }     // Valor del factor
        public string UPosteo { get; set; }           // Usuario que registra
        public DateTime? FPosteo { get; set; }        // Fecha del registro
        public string PC { get; set; }                // Nombre del equipo
        public bool? Estado { get; set; }             // Estado activo/inactivo
    }
    public class Factura2DTO50
    {
        public string Opcion { get; set; }
        public string Factura { get; set; }
        public string ClienteRemitente { get; set; }
        public string ClienteDestino { get; set; }
        public decimal FactorDolar { get; set; }
        public string Guia { get; set; }
        public string Empresa { get; set; }
        public string UPosteo { get; set; }
        public DateTime? FPosteo { get; set; }
        public string PC { get; set; }
        public string Observacion { get; set; }
    }
    public class SucursalUbicacionDTO
    {
        public string Opcion { get; set; }            // Acción a ejecutar en el SP (AGREGAR, ACTUALIZAR, ELIMINAR, LISTADO, RECUPERAR)
        public int? UbicacionID { get; set; }         // ID de la ubicación (clave primaria)
        public string Descripcion { get; set; }       // Nombre o detalle de la ubicación
        public string UPosteo { get; set; }           // Usuario que registra
        public DateTime? FPosteo { get; set; }        // Fecha del registro
        public string PC { get; set; }                // Nombre del equipo
        public bool? Estado { get; set; }             // Estado activo/inactivo
    }

}
