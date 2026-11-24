using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CargaBaseDatosTrasaccional.Validacion
{
    public static class Reglas
    {
        public static string? Obligatorio(string valor, string campo)
            => string.IsNullOrWhiteSpace(valor) ? $"{campo} es obligatorio" : null;

        public static string? LongitudMaxima(string valor, int max, string campo)
          => valor.Length > max ? $"{campo} supera {max} caracteres" : null;

        public static string? EsEntero(string valor, string campo, out int numero)
            => int.TryParse(valor, out numero) ? null : $"{campo} no es un entero valido";

        public static string? EnteroEnRango(int numero, int min, int max, string campo)
            => numero < min || numero > max ? $"{campo} debe estar entre {min} y {max}" : null;

        public static string? FechaValida(string valor, string campo, out DateTime fecha)
                => DateTime.TryParseExact(valor, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha)
                ? null
                : $"{campo} no es una fecha valida (yyyy-MM-dd)";

        public static string? NormalizarId(string valor, char prefijo)
        {
            valor = valor.Trim();

            if (valor.Length > 0 && char.ToUpper(valor[0]) == prefijo)
                valor = valor.Substring(1);

            return int.TryParse(valor, out int n) ? n.ToString() : valor;
        }

        public static string? ExisteEn(HashSet<string> validos, string valor, string campo, string tabla)
            => validos.Contains(valor) ? null : $"{campo} '{valor}' no existe en {tabla}";

        public static string? AlMenosUno(string valorA, string valorB, string campoA, string campoB)
            => Obligatorio(valorA, campoA) != null && Obligatorio(valorB, campoB) != null
            ? $"Debe  venir {campoA} o {campoB}"
            : null;

        public static string? NoRepetido(HashSet<string> vistos, string valor, string campo)
            => vistos.Contains(valor) ? $"{campo} '{valor}' esta repetido" : null;

        private static readonly Regex PatronEmail = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        public static string? EsEmail(string valor, string campo)
            => PatronEmail.IsMatch(valor) ? null : $"{campo} no tiene formato de email valido";

        public static string? ValorPermitido (string valor, string[]permitidos, string campo) =>
            permitidos.Contains(valor) ? null : $"{campo} '{valor}' no es valido (permitidos: {string.Join("," , permitidos)})";

        public static string? ClasicacionCoherente(string Clasificacion, int puntaje) =>
            Clasificacion switch
            {
                "Negativa" when puntaje is < 1 or > 2 => "Clasificacion Negativo requiere puntaje 1 o 2",
                "Neutra" when puntaje != 3 => "Clasificacion Neutra requiere un puntaje de 3",
                "Positiva" when puntaje is < 4 or > 5 => "Clasificacion Negativo requiere puntaje 4 o 5",
                _ => null
            };
    }
}
