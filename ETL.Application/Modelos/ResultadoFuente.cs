using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Aplicacion.Modelos
{
    public class ResultadoFuente
    {
        public string Fuente { get; set; } = string.Empty;
        public int RegistrosCargados { get; set; } 
        public TimeSpan Duracion {  get; set; }
        public bool Exitoso { get; set; }
        public string? Error { get; set; }
    }
}
