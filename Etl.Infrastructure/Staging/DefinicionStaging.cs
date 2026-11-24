using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Infraestructura.Staging
{
    public class DefinicionStaging<T>
    {
        public required string Tabla { get; init; }
        public required (string Nombre, Type Tipo)[] Columnas { get; init; }
        public required Func<T, object?[]> ObtenerValores { get; init; }
    }
}
