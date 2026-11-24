using Etl.Dominio.Entidades;
using Etl.Dominio.Interfaces;
using Etl.Infraestructura.Configuracion;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Infraestructura.Extractores
{
    public class DatabaseExtractor : IExtractor<ResenaWeb>
    {
        private readonly OpcionesBaseDatosOrigen _opciones;
        private readonly ILogger<DatabaseExtractor> _logger;

        public DatabaseExtractor(IOptions<OpcionesBaseDatosOrigen> opciones, ILogger<DatabaseExtractor> logger)
        {
            _opciones = opciones.Value;
            _logger = logger;
        }

        public string NombreFuente => "Reseña web (Base de datos)";

        public async IAsyncEnumerable<ResenaWeb> ExtraerAsync(
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            _logger.LogInformation("Consultando reseñas en la base de datos relacional");

            await using var conexion = new SqlConnection(_opciones.CadenaConexion);
            await conexion.OpenAsync(ct);

            await using var comando = new SqlCommand(_opciones.Consulta, conexion);
            await using var lector = await comando.ExecuteReaderAsync(ct);

            while (await lector.ReadAsync(ct))
            {
                yield return new ResenaWeb
                {
                    IdReview = lector.GetString(0),
                    IdCliente = lector.IsDBNull(1) ? null : lector.GetString(1),
                    IdProducto = lector.IsDBNull(2) ? null : lector.GetString(2),
                    Fecha = lector.IsDBNull(3) ? null : lector.GetDateTime(3),
                    Comentario = lector.IsDBNull(4) ? null : lector.GetString(4),
                    Rating = lector.IsDBNull(5) ? null : lector.GetInt32(5)
                };
            }
        }
    }
}
