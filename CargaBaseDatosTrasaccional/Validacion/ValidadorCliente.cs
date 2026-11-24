using CargaBaseDatosTrasaccional.Entidates;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CargaBaseDatosTrasaccional.Validacion
{
    public static class ValidadorCliente //Valida los datos de un cliente
    {
        public static ResultadoFila<Cliente> Validar (Dictionary<string, string> fila, HashSet<string> idsCargados)
        {
            var r = new ResultadoFila<Cliente>();

            string id = fila["IdCliente"];
            string nombre = fila["Nombre"];
            string email = fila["Email"];

            r.Agregar(Reglas.Obligatorio(id, "IdCliente"));
            r.Agregar(Reglas.EsEntero(id, "IdCliente", out _));
            r.Agregar(Reglas.NoRepetido(idsCargados, id, "IdCliente"));

            r.Agregar(Reglas.Obligatorio(nombre, "Nombre"));
            r.Agregar(Reglas.LongitudMaxima(nombre, 100, "Nombre"));

            r.Agregar(Reglas.Obligatorio(email, "Email"));
            r.Agregar(Reglas.LongitudMaxima(email, 100, "Email"));
            r.Agregar(Reglas.EsEmail(email, "Email"));

            if (r.EsValida)
              r.Entidad = new Cliente {IdCliente = id, Nombre = nombre, Email = email};

            return r;
        }
    }
}
