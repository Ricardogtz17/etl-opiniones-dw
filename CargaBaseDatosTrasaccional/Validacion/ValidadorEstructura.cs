using System;
using System.Collections.Generic;
using System.Text;

namespace CargaBaseDatosTrasaccional.Validacion
{
    public class ValidadorEstructura
    {
        public static string? Validar (List<Dictionary<string, string>> filas, string[]columnasEsperadas, string archivo)
        {
            if (filas.Count == 0)
                return $"{archivo}: el archivo esta vacio o no tiene fila de datos";

            var faltantes = columnasEsperadas.Except(filas[0].Keys).ToList();

            return faltantes.Any()
                ? $"{archivo}: faltan las columnas {string.Join(",", faltantes)}"
                : null;
        }
    }
}
