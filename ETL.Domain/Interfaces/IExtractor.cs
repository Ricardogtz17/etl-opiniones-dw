using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Dominio.Interfaces
{
    public interface IExtractor<T>
    {
        string NombreFuente { get; }
        IAsyncEnumerable<T> ExtraerAsync(CancellationToken ct = default);
    }
}
