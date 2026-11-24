using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CargaBaseDatosTrasaccional.Servicios
{
    public static class ReporteFinal
    {
        public static void Imprimir(List<ResultadoTabla> resultados, string archivoRechazos)
        {
            Console.WriteLine();
            Console.WriteLine("=== RESUMEN POR TABLA ===");
            foreach (var r in resultados)
            {
                if (r.ErrorEstructura != null)
                    Console.WriteLine($"{r.Tabla,-16} ARCHIVO RECHAZADO -> {r.ErrorEstructura}");
                else
                    Console.WriteLine($"{r.Tabla,-16} leídos {r.Leidos,4} | insertados {r.Insertados,4} | ya existían {r.YaExistian,4} | rechazados {r.Rechazados,4}");
            }

            Console.WriteLine();
            Console.WriteLine("=== TOTALES ===");
            Console.WriteLine($"Leídos {resultados.Sum(r => r.Leidos)} | " +
                              $"Insertados {resultados.Sum(r => r.Insertados)} | " +
                              $"Ya existían {resultados.Sum(r => r.YaExistian)} | " +
                              $"Rechazados {resultados.Sum(r => r.Rechazados)}");

            var conRechazos = resultados.Where(r => r.Rechazos.Count > 0).ToList();
            if (conRechazos.Count == 0) return;

            Console.WriteLine();
            Console.WriteLine("=== MUESTRA DE RECHAZOS (3 por tabla) ===");
            foreach (var r in conRechazos)
            {
                Console.WriteLine($"{r.Tabla} ({r.Rechazados} rechazadas):");
                foreach (var motivo in r.Rechazos.Take(3))
                    Console.WriteLine($"   {motivo}");
            }

            File.WriteAllLines(archivoRechazos,
                resultados.SelectMany(r => r.Rechazos.Select(m => $"[{r.Tabla}] {m}")));
            Console.WriteLine();
            Console.WriteLine($"Detalle completo en: {archivoRechazos}");
        }
    }
}
