using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Infraestructura.Utilidades
{
    public static class UtilidadesTexto
    {
        public static string Normalizar(string texto)
        {
            var descompuesto = texto.Trim().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in descompuesto)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        }
    }
}
