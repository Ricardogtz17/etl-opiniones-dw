using CargaBaseDatosTrasaccional.Entidates;
using System;
using System.Collections.Generic;
using System.Text;

namespace CargaBaseDatosTrasaccional.Validacion
{
    public class ValidadorSocialComment
    {
        public static ResultadoFila<SocialComment> Validar
            (
            Dictionary<string, string> fila,
            HashSet<string> idsCargados,
            HashSet<string> idsClientes,
            HashSet<string> idsProductos
            )
        {
            var r = new ResultadoFila<SocialComment>();

            string id = fila["IdComment"];
            string idClienteTexto = Reglas.NormalizarId(fila["IdCliente"], 'C');
            string idProducto = Reglas.NormalizarId(fila["IdProducto"], 'P');
            string fuente = fila["Fuente"];
            string fechaTexto = fila["Fecha"];
            string comentario = fila["Comentario"];

            r.Agregar(Reglas.Obligatorio(id, "IdComment")
                ?? Reglas.LongitudMaxima(id, 50, "IdComment")
                ?? Reglas.NoRepetido(idsCargados, id, "IdComment"));

            string? idCliente = null;
            if (!string.IsNullOrWhiteSpace(idClienteTexto))
            {
                r.Agregar(Reglas.EsEntero(idClienteTexto, "IdCliente", out _)
                    ?? Reglas.ExisteEn(idsClientes,idClienteTexto, "IdCliente", "Clientes"));
                idCliente = idClienteTexto;
            }

            r.Agregar(Reglas.Obligatorio(idProducto, "IdProducto")
               ?? Reglas.EsEntero(idProducto, "IdProducto", out _)
               ?? Reglas.ExisteEn(idsProductos, idProducto, "IdProducto", "Productos"));

            r.Agregar(Reglas.Obligatorio(fuente, "Fuente")
                ?? Reglas.LongitudMaxima(fuente, 50, "Fuente"));

            DateTime fecha = default;
            r.Agregar(Reglas.Obligatorio(fechaTexto, "Fecha")
                ??Reglas.FechaValida(fechaTexto, "Fecha", out fecha));

            r.Agregar(Reglas.Obligatorio(comentario, "Comentario"));

            if (r.EsValida)
            {
                r.Entidad = new SocialComment
                {
                    IdComment = id,
                    IdCliente = idCliente,
                    IdProducto = idProducto,
                    Fuente = fuente,
                    Fecha = fecha,
                    Comentario = comentario
                };
            }

            return r;
        }
    }
}
