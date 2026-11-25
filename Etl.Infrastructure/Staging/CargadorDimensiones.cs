using Etl.Aplicacion.Interfaces;
using Etl.Infraestructura.Configuracion;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Infraestructura.Staging
{
    public class CargadorDimensiones : ICargadorDimensiones
    {
        private readonly OpcionesStaging _opciones;
        private readonly ILogger<CargadorDimensiones> _logger;

        public CargadorDimensiones(IOptions<OpcionesStaging> opciones, ILogger<CargadorDimensiones> logger)
        {
            _opciones = opciones.Value;
            _logger = logger;
        }

        public async Task<IReadOnlyDictionary<string, int>> CargarAsync(CancellationToken ct = default)
        {
            var conteos =  new Dictionary<string, int>();

            await using var conexion = new SqlConnection(_opciones.CadenaConexion);
            await conexion.OpenAsync();

            await using var comando = new SqlCommand("dbo.usp_CargarDimensiones", conexion)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandTimeout = 120
            };

            await using var lector = await comando.ExecuteReaderAsync(ct);
            while (await lector.ReadAsync(ct))
                conteos[lector.GetString(0)] = lector.GetInt32(1);

            _logger.LogInformation("Carga de dimensiones terminada ({Total} tablas)", conteos.Count);
            return conteos;
        }
    }
}
