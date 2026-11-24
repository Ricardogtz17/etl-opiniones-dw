using System;
using System.Collections.Generic;
using System.Text;

namespace CargaBaseDatosTrasaccional.Servicios
{
    public class ResultadoTabla
    {
        public string Tabla { get; }
        public int Leidos { get; set; }
        public int Insertados {  get; set; }
        public int YaExistian {  get; set; }
        public string? ErrorEstructura { get; set; }
        public List<string> Rechazos { get; } = new();
        public int Rechazados => Rechazos.Count;

        public ResultadoTabla(string tabla) => Tabla = tabla;

        public void Rechazar (int fila, IEnumerable<string> errores) =>
            Rechazos.Add($"Fila{fila}: {string.Join("|", errores)}");
    }
}
