using CargaBaseDatosTrasaccional.Entidates;
using System;
using System.Collections.Generic;
using System.Text;

namespace CargaBaseDatosTrasaccional.Validacion
{
    public class ValidadorProducto //valida los datos de un producto.
    {
        public static ResultadoFila<Producto> Validar (Dictionary<string, string> fila, HashSet<string> idsCargados)
        {
            var r = new ResultadoFila<Producto>();

            string id = fila["IdProducto"];
            string nombre = fila["Nombre"];
            string categoria = fila["Categoria"];

            r.Agregar(Reglas.Obligatorio(id, "IdProducto"));
            r.Agregar(Reglas.EsEntero(id, "IdProducto", out _));
            r.Agregar(Reglas.NoRepetido(idsCargados, id,"IdProducto"));

            r.Agregar(Reglas.Obligatorio(nombre, "Nombre"));
            r.Agregar(Reglas.LongitudMaxima(nombre, 100, "Nombre"));

            r.Agregar(Reglas.Obligatorio(categoria, "Categoria"));
            r.Agregar(Reglas.LongitudMaxima(categoria,100, "Categoria"));

            if (r.EsValida)
                r.Entidad = new Producto { IdProducto = id, Nombre = nombre, Categoria = categoria };

            return r;
        }
    }
}
