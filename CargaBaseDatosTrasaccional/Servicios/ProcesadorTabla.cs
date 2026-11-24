using System;
using System.Collections.Generic;
using CargaBaseDatosTrasaccional.AccesoDatos;
using CargaBaseDatosTrasaccional.Extraccion;
using CargaBaseDatosTrasaccional.Validacion;
using Microsoft.Data.SqlClient;
using System.Text;

namespace CargaBaseDatosTrasaccional.Servicios
{
    public class ProcesadorTabla
    {
        private readonly LectorCsv _lector;
        private readonly EjecutorProcedimientos _db;
        private readonly string _rutaDatos;

        public ProcesadorTabla(LectorCsv lector, EjecutorProcedimientos db, string rutaDatos)
        {
            _lector = lector;
            _db = db;
            _rutaDatos = rutaDatos;
        }

        public ResultadoTabla Procesar<T>(DefinicionTabla<T> def) where T : class
        {
            var resultado = new ResultadoTabla(def.Nombre);
            string ruta = Path.Combine(_rutaDatos, def.Archivo);

            if (!File.Exists(ruta))
            {
                resultado.ErrorEstructura = $"{def.Archivo}: no se encontró el archivo";
                return resultado;
            }

            var filas = _lector.Leer(ruta);

            resultado.ErrorEstructura = ValidadorEstructura.Validar(filas, def.ColumnasEsperadas, def.Archivo);
            if (resultado.ErrorEstructura != null)
                return resultado;

            resultado.Leidos = filas.Count;

            for (int i = 0; i < filas.Count; i++) 
            {
                int numeroFila = i + 2;
                var r = def.Validar(filas[i]);

                if (!r.EsValida)
                {
                    resultado.Rechazar(numeroFila, r.Errores);
                    continue;
                }

                try
                {
                    bool inserto = _db.Ejecutar(def.Procedimiento, def.ObtenerParametros(r.Entidad));
                    if (inserto) resultado.Insertados++;
                    else resultado.YaExistian++;

                    def.IdsCargados.Add(def.ObtenerId(r.Entidad!));
                }
                catch (SqlException ex)
                {
                    resultado.Rechazar(numeroFila, new[] { $"Error de base de datos: {ex.Message}" });
                }
            }
            return resultado;
        }
    }
}
