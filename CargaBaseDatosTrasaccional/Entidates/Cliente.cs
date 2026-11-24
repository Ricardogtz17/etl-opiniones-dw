using System;
using System.Collections.Generic;
using System.Text;

namespace CargaBaseDatosTrasaccional.Entidates
{
    public class Cliente
    {
        public string IdCliente { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
