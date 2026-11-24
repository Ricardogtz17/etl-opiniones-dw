using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Dominio.Entidades
{
    public class ResenaWeb
    {
        public string IdReview { get; set; } = string.Empty;
        public string? IdCliente {  get; set; } 
        public string? IdProducto { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Comentario { get; set; }
        public int? Rating { get; set; }
    }
}
