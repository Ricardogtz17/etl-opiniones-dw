using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Etl.Dominio.Entidades;

namespace Etl.Infraestructura.Staging;

public static class DefinicionesStaging
{
    public static DefinicionStaging<EncuestaInterna> Encuestas => new()
    {
        Tabla = "stg.encuesta_interna",
        Columnas = new (string, Type)[]
        {
            ("id_opinion", typeof(string)), ("id_cliente", typeof(string)),
            ("id_producto", typeof(string)), ("fecha", typeof(DateTime)),
            ("comentario", typeof(string)), ("clasificacion", typeof(string)),
            ("puntaje_satisfaccion", typeof(int)), ("fuente", typeof(string))
        },
        ObtenerValores = e => new object?[]
        {
            e.IdOpinion, e.IdCliente, e.IdProducto, e.Fecha,
            e.Comentario, e.Clasificacion, e.PuntajeSatisfaccion, e.Fuente
        }
    };

    public static DefinicionStaging<ResenaWeb> Resenas => new()
    {
        Tabla = "stg.resena_web",
        Columnas = new (string, Type)[]
        {
            ("id_review", typeof(string)), ("id_cliente", typeof(string)),
            ("id_producto", typeof(string)), ("fecha", typeof(DateTime)),
            ("comentario", typeof(string)), ("rating", typeof(int))
        },
        ObtenerValores = r => new object?[]
        {
            r.IdReview, r.IdCliente, r.IdProducto, r.Fecha, r.Comentario, r.Rating
        }
    };

    public static DefinicionStaging<ComentarioSocial> Comentarios => new()
    {
        Tabla = "stg.comentario_social",
        Columnas = new (string, Type)[]
        {
            ("id_comment", typeof(string)), ("id_cliente", typeof(string)),
            ("id_producto", typeof(string)), ("fuente", typeof(string)),
            ("fecha", typeof(DateTime)), ("comentario", typeof(string))
        },
        ObtenerValores = c => new object?[]
        {
            c.IdComment, c.IdCliente, c.IdProducto, c.Fuente, c.Fecha, c.Comentario
        }
    };
}