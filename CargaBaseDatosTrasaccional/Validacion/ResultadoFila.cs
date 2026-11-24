using System;
using System.Collections.Generic;
using System.Text;

namespace CargaBaseDatosTrasaccional.Validacion
{
    public class ResultadoFila <T> where T : class
    {
        public T? Entidad { get; set; }
        public List<string> Errores { get; set; } = new();
        public bool EsValida => Errores.Count == 0;
        public void Agregar (string? error)
        {
            if (error != null) Errores.Add(error);
        }
    }
}
