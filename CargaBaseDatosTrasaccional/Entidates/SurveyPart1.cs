using System;

namespace CargaBaseDatosTrasaccional.Entidates
{
    public class SurveyPart1
    {
        public string IdOpinion {  get; set; } = string.Empty;
        public string IdCliente { get; set; } = string.Empty;
        public string IdProducto {  get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Comentario {  get; set; } = string.Empty;
        public string Clasificacion { get; set; } = string.Empty;
        public int PuntajeSatisfaccion {  get; set; }
        public string Fuente { get; set; } = string.Empty;
    }
}
