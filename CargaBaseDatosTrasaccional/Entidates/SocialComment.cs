using System;
using System.Collections.Generic;
using System.Text;

namespace CargaBaseDatosTrasaccional.Entidates
{
    public class SocialComment
    {
        public string IdComment { get; set; } = string.Empty;
        public string? IdCliente { get; set; } = string.Empty;
        public string IdProducto { get; set; } = string.Empty;
        public string Fuente { get; set; } = string.Empty;
        public string Comentario { get; set; } = string.Empty;
        public DateTime Fecha {  get; set; } 

    }
}
