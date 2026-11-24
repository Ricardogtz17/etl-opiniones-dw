using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace CargaBaseDatosTrasaccional.AccesoDatos
{
    public class EjecutorProcedimientos : IDisposable
    {
        private readonly SqlConnection _conexion;

        public EjecutorProcedimientos(string cadenaConexion)
        {
            _conexion = new SqlConnection(cadenaConexion);
            _conexion.Open();
        }

        public bool Ejecutar(string procedimiento, params (string Nombre, object? Valor)[] parametros)
        {
            using var comando = new SqlCommand(procedimiento, _conexion)
            {
                CommandType = CommandType.StoredProcedure
            };

            foreach (var (nombre, valor) in parametros)
                comando.Parameters.AddWithValue(nombre, valor ?? DBNull.Value);

            return comando.ExecuteNonQuery() > 0;
        }

        public void Dispose() => _conexion.Dispose();
    }
}
