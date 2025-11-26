using ModuloCajaRC.Facturas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuloCajaRC
{
    public class ControlCaja
    {
        private readonly IMainContext _context;

        public ControlCaja(IMainContext context)
        {
            _context = context;
        }

        public void ProcesarControl(string Url,string TipoSeguimiento, int IDSeguimiento,int cajaID)
        {
            _context.ActualizarUbicacion(Url);
            _context.SeguimientoUsuario("INSERTAR", IDSeguimiento);
            _context.cajaID = cajaID;
        }
    }
}
