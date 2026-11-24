using CsvHelper;
using CsvHelper.Configuration;
using Etl.Dominio.Entidades;
using Etl.Dominio.Interfaces;
using Etl.Infraestructura.Configuracion;
using Etl.Infraestructura.Utilidades;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Infraestructura.Extractores
{
    public class CsvExtractor : IExtractor<EncuestaInterna>
    {
        private readonly OpcionesCsv _opciones;
        private readonly ILogger <CsvExtractor> _logger;

        public CsvExtractor (IOptions<OpcionesCsv> opciones, ILogger<CsvExtractor> logger)
        {
            _opciones = opciones.Value;
            _logger = logger;
        }

        public string NombreFuente => "Encuesta Interna (CSV)";

        public async IAsyncEnumerable<EncuestaInterna> ExtraerAsync(
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            if (!File.Exists(_opciones.RutaArchivo))
                throw new FileNotFoundException("No se encontro el csv de la encuesta.", _opciones.RutaArchivo);

            _logger.LogInformation("Leyendo CSV: {Ruta}", _opciones.RutaArchivo);

            var configuracion = new CsvConfiguration (CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => UtilidadesTexto.Normalizar(args.Header),
                MissingFieldFound = null,
                ReadingExceptionOccurred = args =>
                {
                    _logger.LogWarning("Fila {Fila} omitida: {Mensaje}",
                        args.Exception.Context?.Parser?.Row, args.Exception.Message);
                    return false;
                }
            };

            using var lector = new StreamReader (_opciones.RutaArchivo, Encoding.UTF8);
            using var csv = new CsvReader(lector, configuracion);

            await foreach (var fila in csv.GetRecordsAsync<EncuestaInterna>(ct))
                yield return fila;
        }
            
    }
}
