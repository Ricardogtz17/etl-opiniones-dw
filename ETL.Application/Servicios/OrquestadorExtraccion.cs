using Etl.Aplicacion.Modelos;
using Etl.Aplicacion.Interfaces;
using Etl.Dominio.Entidades;
using Etl.Dominio.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Aplicacion.Servicios
{
    public class OrquestadorExtraccion
    {
        private readonly IExtractor<EncuestaInterna> _extractorEncuestas;
        private readonly IExtractor<ResenaWeb> _extractorResenas;
        private readonly IExtractor<ComentarioSocial> _extractorComentarios;
        private readonly IDataLoader<EncuestaInterna> _cargadorEncuestas;
        private readonly IDataLoader<ResenaWeb> _cargadorResenas;
        private readonly IDataLoader<ComentarioSocial> _cargadorComentarios;
        private readonly ILogger<OrquestadorExtraccion> _logger;

        public OrquestadorExtraccion(
       IExtractor<EncuestaInterna> extractorEncuestas,
       IExtractor<ResenaWeb> extractorResenas,
       IExtractor<ComentarioSocial> extractorComentarios,
       IDataLoader<EncuestaInterna> cargadorEncuestas,
       IDataLoader<ResenaWeb> cargadorResenas,
       IDataLoader<ComentarioSocial> cargadorComentarios,
       ILogger<OrquestadorExtraccion> logger)
        {
            _extractorEncuestas = extractorEncuestas;
            _extractorResenas = extractorResenas;
            _extractorComentarios = extractorComentarios;
            _cargadorEncuestas = cargadorEncuestas;
            _cargadorResenas = cargadorResenas;
            _cargadorComentarios = cargadorComentarios;
            _logger = logger;
        }

        public async Task<List<ResultadoFuente>> EjecutarAsync(CancellationToken ct = default)
        {
            var loteId = Guid.NewGuid();
            var reloj = Stopwatch.StartNew();
            _logger.LogInformation("Inicia la extracción. Lote {LoteId}", loteId);

            var tareas = new[]
            {
            ProcesarAsync(_extractorEncuestas, _cargadorEncuestas, loteId, ct),
            ProcesarAsync(_extractorResenas, _cargadorResenas, loteId, ct),
            ProcesarAsync(_extractorComentarios, _cargadorComentarios, loteId, ct)
        };
            var resultados = await Task.WhenAll(tareas);

            reloj.Stop();
            _logger.LogInformation(
                "Extracción terminada en {Segundos:F2} s. Registros cargados: {Total}",
                reloj.Elapsed.TotalSeconds, resultados.Sum(r => r.RegistrosCargados));

            return resultados.ToList();
        }

        private async Task<ResultadoFuente> ProcesarAsync<T>(
            IExtractor<T> extractor, IDataLoader<T> cargador, Guid loteId, CancellationToken ct)
        {
            var resultado = new ResultadoFuente { Fuente = extractor.NombreFuente };
            var reloj = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Extrayendo: {Fuente}", extractor.NombreFuente);
                resultado.RegistrosCargados = await cargador.CargarAsync(extractor.ExtraerAsync(ct), loteId, ct);
                resultado.Exitoso = true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                resultado.Error = ex.Message;
                _logger.LogError(ex, "Falló la extracción de {Fuente}", extractor.NombreFuente);
            }

            reloj.Stop();
            resultado.Duracion = reloj.Elapsed;
            return resultado;
        }
    }
}
