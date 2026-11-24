using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Infraestructura.Configuracion;

public class OpcionesBaseDatosOrigen
{
    public string CadenaConexion { get; set; } = string.Empty;
    public string Consulta { get; set; } =
        "SELECT IdReview, IdCliente, IdProducto, Fecha, Comentario, Rating FROM Web_Reviews";
}

