using Etl.Dominio.Entidades;
using Etl.Dominio.Interfaces;
using Etl.Infraestructura.Configuracion;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Infraestructura.Extractores
{
    public class ApiExtractor : IExtractor<ComentarioSocial>
    {
        private readonly IHttpClientFactory _fabrica;
        private readonly OpcionesApi _opciones;
        private readonly ILogger<ApiExtractor> _logger;

        public ApiExtractor(IHttpClientFactory fabrica, IOptions<OpcionesApi> opciones, ILogger<ApiExtractor> logger)
        {
            _fabrica = fabrica;
            _opciones = opciones.Value;
            _logger = logger;
        }

        public string NombreFuente => "Comentario social (API REST)";

        public async IAsyncEnumerable<ComentarioSocial> ExtraerAsync(
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            var cliente = _fabrica.CreateClient("ApiComentarios");
            int pagina = 1;

            while (true)
            {
                var url = $"{_opciones.Ruta}?pagina={pagina}&tamanoPagina={_opciones.TamanoPagina}";
                _logger.LogInformation("Consultando API: {Url}", url);

                var respuesta = await cliente.GetFromJsonAsync<RespuestaPaginada>(url, ct);
                if (respuesta is null || respuesta.Datos.Count == 0)
                    yield break;

                foreach (var comentario in respuesta.Datos)
                    yield return comentario;

                if (pagina * _opciones.TamanoPagina >= respuesta.Total)
                    yield break;

                pagina++;
            }
        }
    }

    public class RespuestaPaginada
    {
        public int Total { get; set; }
        public List<ComentarioSocial> Datos { get; set; } = new();
    }
}
