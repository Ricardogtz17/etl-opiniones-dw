using CargaBaseDatosTrasaccional.Entidates;
using System;
using System.Collections.Generic;
using System.Text;

namespace CargaBaseDatosTrasaccional.Validacion
{
    public class ValidadorWebReview //valida una opinión de un cliente y sus relaciones.
    {
        public static ResultadoFila<WebReview> Validar(
            Dictionary<string, string> fila,
            HashSet<string> idsCargados,
            HashSet<string> idsClientes,
            HashSet<string> idsProductos)
        {
            var r = new ResultadoFila<WebReview>();

            string id = fila["IdReview"];
            string idCliente = Reglas.NormalizarId(fila["IdCliente"], 'C');
            string idProducto = Reglas.NormalizarId(fila["IdProducto"], 'P');
            string fechaTexto = fila["Fecha"];
            string comentario = fila["Comentario"];
            string ratingTexto = fila["Rating"];

            r.Agregar(Reglas.Obligatorio(id, "IdReview")
                ?? Reglas.LongitudMaxima(id, 50, "IdReview")
                ?? Reglas.NoRepetido(idsCargados, id, "IdReview"));

            r.Agregar(Reglas.Obligatorio(idCliente, "IdCliente")
                ?? Reglas.EsEntero(idCliente, "IdCliente", out _)
                ?? Reglas.ExisteEn(idsClientes, idCliente, "IdCliente", "Clientes"));

            r.Agregar(Reglas.Obligatorio(idProducto, "IdProducto")
                  ?? Reglas.EsEntero(idProducto, "IdProducto", out _)
                  ?? Reglas.ExisteEn(idsProductos, idProducto, "IdProducto", "Productos"));

            DateTime fecha = default;
            r.Agregar(Reglas.Obligatorio(fechaTexto, "Fecha")
                ?? Reglas.FechaValida(fechaTexto, "Fecha", out fecha));

            int rating = 0;
            if (!string.IsNullOrWhiteSpace(ratingTexto))
            {
                string? errRating = Reglas.EsEntero(ratingTexto, "Rating", out int valor)
                    ?? Reglas.EnteroEnRango(valor, 1, 5, "Rating");
                r.Agregar(errRating);
                rating = valor;
            }

            r.Agregar(Reglas.AlMenosUno(comentario, ratingTexto, "Comentario", "Rating"));

            if (r.EsValida)
            {
                r.Entidad = new WebReview
                {
                    IdReview = id,
                    IdCliente= idCliente,
                    IdProducto = idProducto,
                    Fecha = fecha,
                    Comentario = comentario,
                    Rating = rating
                };
            }

            return r;

        } 
    }
}
