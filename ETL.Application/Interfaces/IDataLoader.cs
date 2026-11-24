using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Aplicacion.Interfaces
{
    public interface IDataLoader<T>
    {
        Task<int> CargarAsync(IAsyncEnumerable<T> registros, Guid loteId, CancellationToken ct = default);
    }
}
