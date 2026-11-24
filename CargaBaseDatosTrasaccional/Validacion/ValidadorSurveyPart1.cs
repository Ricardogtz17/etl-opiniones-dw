using CargaBaseDatosTrasaccional.Entidates;
using System;
using System.Collections.Generic;
using System.Text;

namespace CargaBaseDatosTrasaccional.Validacion
{
    public class ValidadorSurveyPart1
    {
        private static readonly string[] Clasificaciones = { "Positiva", "Neutra", "Negativa" };
        public static ResultadoFila<SurveyPart1> Validar
            (
            Dictionary<string, string> fila,
            HashSet<string> idsCargados,
            HashSet<string> idsCliente,
            HashSet<string> idsProductos
            )
        {
            var r = new ResultadoFila<SurveyPart1>();

            string id = fila["IdOpinion"];
            string idCliente = Reglas.NormalizarId(fila["IdCliente"], 'C');
            string idProducto = Reglas.NormalizarId(fila["IdProducto"], 'P');
            string fechaTexto = fila["Fecha"];
            string comentario = fila["Comentario"];
            string clasificacion = fila["Clasificacion"];
            string satisfaccion = fila["PuntajeSatisfaccion"];
            string fuente = fila["Fuente"];

            r.Agregar(Reglas.Obligatorio(id, "IdOpinion")
                 ?? Reglas.LongitudMaxima(id, 50, "IdOpinion")
                 ?? Reglas.NoRepetido(idsCargados, id, "IdOpinion"));

            r.Agregar(Reglas.Obligatorio(idCliente, "IdCliente")
                ?? Reglas.EsEntero(idCliente, "IdCliente", out _)
                ?? Reglas.ExisteEn(idsCliente, idCliente, "IdCliente", "Clientes"));

            r.Agregar(Reglas.Obligatorio(idProducto, "IdProducto")
                  ?? Reglas.EsEntero(idProducto, "IdProducto", out _)
                  ?? Reglas.ExisteEn(idsProductos, idProducto, "IdProducto", "Productos"));

            DateTime fecha = default;
            r.Agregar(Reglas.Obligatorio(fechaTexto, "Fecha")
                ?? Reglas.FechaValida(fechaTexto, "Fecha", out fecha));


            r.Agregar(Reglas.Obligatorio(clasificacion, "Clasificacion")
                ?? Reglas.ValorPermitido(clasificacion,  Clasificaciones, "Clasificacion"));


            r.Agregar(Reglas.Obligatorio(fuente, "Fuente")
               ?? Reglas.LongitudMaxima(fuente, 50, "Fuente"));

            int puntaje = 0;
            if (r.EsValida)
            {
                r.Agregar(Reglas.Obligatorio(satisfaccion, "PuntajeSatisfaccion" )
                    ?? Reglas.EsEntero(satisfaccion, "PuntajeSatisfaccion", out puntaje)
                    ?? Reglas.EnteroEnRango(puntaje, 1, 5, "PuntajeSatisfaccion"));
            }

            r.Agregar(Reglas.Obligatorio(fuente, "Fuente")
               ?? Reglas.LongitudMaxima(fuente, 50, "Fuente"));

            if (r.EsValida)
                r.Agregar(Reglas.ClasicacionCoherente(clasificacion, puntaje));

            if (r.EsValida)
            {
                r.Entidad = new SurveyPart1
                {
                    IdOpinion = id,IdCliente = idCliente, IdProducto = idProducto,
                    Fecha = fecha,Comentario = comentario,
                    Clasificacion = clasificacion,PuntajeSatisfaccion = puntaje, Fuente = fuente,
                };
            }

            return r;
        }

        
    }

    
}
