using System;
using System.Collections.Generic;
using System.Text;

namespace CargaBaseDatosTrasaccional.Entidates
{
    public class WebReview
    {
        public string IdReview { get; set; } = string.Empty;
        public string IdCliente { get; set; } = string.Empty;
        public string IdProducto { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Comentario { get; set; }= string.Empty;
        public int Rating { get; set; }
    }
}
