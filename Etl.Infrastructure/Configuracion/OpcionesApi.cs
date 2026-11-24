using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Infraestructura.Configuracion;

public class OpcionesApi
{
    public string UrlBase { get; set; } = string.Empty;
    public string Ruta { get; set; } = "/api/comentarios";
    public string ClaveApi { get; set; } = string.Empty;
    public int TamanoPagina { get; set; } = 50;

}
