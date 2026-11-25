using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Aplicacion.Interfaces
{
    public interface ICargadorDimensiones
    {
        Task<IReadOnlyDictionary<string , int >> CargarAsync(CancellationToken ct = default);
    }
}
