using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuloCajaRC.Facturas
{
    public interface IMainContext
    {
        void ActualizarUbicacion(string ubicacion);
        void SeguimientoUsuario(string accion, int codigo);
        int usuarioIDNumber { get; }
        int usuarioAutorizaCierreCaja { get; set; }
        int cajaID { get; set; }
    }
}
