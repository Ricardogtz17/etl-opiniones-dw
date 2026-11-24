using Etl.Aplicacion.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Worker
{
    public class ServicioExtraccion : BackgroundService
    {
        private readonly IServiceScopeFactory _fabricaScopes;
        private readonly IHostApplicationLifetime _ciclo;
        private readonly IConfiguration _configuracion;
        private readonly ILogger<ServicioExtraccion> _logger;

        public ServicioExtraccion(IServiceScopeFactory fabricaScopes, IHostApplicationLifetime ciclo,
                                  IConfiguration configuracion, ILogger<ServicioExtraccion> logger)
        {
            _fabricaScopes = fabricaScopes;
            _ciclo = ciclo;
            _configuracion = configuracion;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int intervaloMinutos = _configuracion.GetValue<int>("Worker:IntervaloMinutos");

            try
            {
                do
                {
                    using var scope = _fabricaScopes.CreateScope();
                    var orquestador = scope.ServiceProvider.GetRequiredService<OrquestadorExtraccion>();
                    var resultados = await orquestador.EjecutarAsync(stoppingToken);

                    foreach (var r in resultados)
                    {
                        if (r.Exitoso)
                            _logger.LogInformation("{Fuente}: {Registros} registros en {Segundos:F2} s",
                                r.Fuente, r.RegistrosCargados, r.Duracion.TotalSeconds);
                        else
                            _logger.LogError("{Fuente}: FALLÓ ({Error})", r.Fuente, r.Error);
                    }

                    if (intervaloMinutos <= 0) break;
                    await Task.Delay(TimeSpan.FromMinutes(intervaloMinutos), stoppingToken);
                }
                while (!stoppingToken.IsCancellationRequested);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Servicio detenido.");
            }

            if (intervaloMinutos <= 0)
                _ciclo.StopApplication();
        }
    }
    
}

