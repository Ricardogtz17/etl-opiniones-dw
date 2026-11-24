using System;
using System.Collections.Generic;
using System.Text;
using CargaBaseDatosTrasaccional.Validacion;

namespace CargaBaseDatosTrasaccional.Servicios
{
    public class DefinicionTabla<T> where T : class
    {
        public required string Nombre { get; set; }
        public required string Archivo { get; set; }
        public required string[] ColumnasEsperadas { get; set; }
        public required HashSet <string> IdsCargados { get; set;  }
        public Func<Dictionary<string,string>, ResultadoFila<T>> Validar { get; init; }
        public required Func<T, string> ObtenerId { get; set;  }
        public required string Procedimiento { get; init; }
        public required Func<T,(string, object?)[]> ObtenerParametros { get; init; }
    }
}
