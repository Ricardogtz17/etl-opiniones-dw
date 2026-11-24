using System;
using System.Collections.Generic;
using System.Text;

namespace CargaBaseDatosTrasaccional.Entidates
{
    public class FuenteDatos
    {
        public string IdFuente {  get; set; } = string.Empty;
        public string TipoFuente { get; set; } = string.Empty;
        public DateTime FechaCarga {  get; set; }
    }
}
