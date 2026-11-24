using CargaBaseDatosTrasaccional.Entidates;
using System;
using System.Collections.Generic;
using System.Text;

namespace CargaBaseDatosTrasaccional.Validacion
{
    public class ValidadorFuenteDatos //valida la información de una fuente de datos.
    {
        public static ResultadoFila<FuenteDatos> Validar(Dictionary<string,string> fila, HashSet<string> idsCargados)
        {
            var r = new ResultadoFila<FuenteDatos>();

            string id = fila["IdFuente"];
            string tipo = fila["TipoFuente"];
            string fechaTexto = fila["FechaCarga"];
            DateTime fecha = default;

            r.Agregar(Reglas.Obligatorio(id, "IdFuente")
                ?? Reglas.LongitudMaxima(id, 10, "IdFuente")
                ?? Reglas.NoRepetido(idsCargados,id,"IdFuente"));

            r.Agregar(Reglas.Obligatorio(tipo, "TipoFuente")
                 ??Reglas.LongitudMaxima(tipo, 50, "TipoFuente"));

            r.Agregar(Reglas.Obligatorio(fechaTexto, "FechaCarga")
            ??Reglas.FechaValida(fechaTexto, "FechaCarga", out fecha));

            if (r.EsValida)
                r.Entidad = new FuenteDatos {IdFuente = id, TipoFuente = tipo, FechaCarga = fecha};

            return r;
        }
    }
}