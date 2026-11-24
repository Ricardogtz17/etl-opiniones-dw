using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace CargaBaseDatosTrasaccional.Extraccion
{
    public class LectorCsv
    {
        private static string QuitarAcentos(string texto)
        {
            string descompuesto = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (char c in descompuesto)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);

        }
        public List<Dictionary<string, string>> Leer(string ruta)
        {
            var filas = new List<Dictionary<string, string>>();

            using (var parser = new TextFieldParser(ruta, System.Text.Encoding.UTF8))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;

                string[]? encabezados = parser.ReadFields();
                if (encabezados == null)
                    throw new InvalidDataException("El archivo esta vacio: " + ruta);

                while (!parser.EndOfData)
                {
                    string[]? campos = parser.ReadFields();
                    if (campos == null) continue;

                    var fila = new Dictionary<string, string>();
                    for (int i = 0; i < encabezados.Length; i++)
                    {
                        fila[QuitarAcentos(encabezados[i].Trim())] = i < campos.Length ? campos[i].Trim() : string.Empty;
                    }

                    filas.Add(fila);
                }
            }

            return filas;

        }
    }
}
