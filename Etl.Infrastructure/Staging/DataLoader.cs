using System.Data;
using Etl.Aplicacion.Interfaces;
using Etl.Infraestructura.Configuracion;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Etl.Infraestructura.Staging;

public class DataLoader<T> : IDataLoader<T>
{
    private const int TamanoLote = 1000;

    private readonly DefinicionStaging<T> _definicion;
    private readonly OpcionesStaging _opciones;
    private readonly ILogger<DataLoader<T>> _logger;

    public DataLoader(DefinicionStaging<T> definicion, IOptions<OpcionesStaging> opciones,
                      ILogger<DataLoader<T>> logger)
    {
        _definicion = definicion;
        _opciones = opciones.Value;
        _logger = logger;
    }

    public async Task<int> CargarAsync(IAsyncEnumerable<T> registros, Guid loteId,
                                       CancellationToken ct = default)
    {
        var tabla = CrearTabla();
        int total = 0;

        await using var conexion = new SqlConnection(_opciones.CadenaConexion);
        await conexion.OpenAsync(ct);

        await foreach (var registro in registros.WithCancellation(ct))
        {
            var valores = _definicion.ObtenerValores(registro);
            var fila = tabla.NewRow();
            for (int i = 0; i < valores.Length; i++)
                fila[i] = valores[i] ?? DBNull.Value;
            fila["lote_id"] = loteId;
            tabla.Rows.Add(fila);

            if (tabla.Rows.Count >= TamanoLote)
                total += await VolcarAsync(conexion, tabla, ct);
        }

        if (tabla.Rows.Count > 0)
            total += await VolcarAsync(conexion, tabla, ct);

        _logger.LogInformation("{Tabla}: {Total} filas insertadas en staging", _definicion.Tabla, total);
        return total;
    }

    private DataTable CrearTabla()
    {
        var tabla = new DataTable();
        foreach (var (nombre, tipo) in _definicion.Columnas)
            tabla.Columns.Add(nombre, tipo);
        tabla.Columns.Add("lote_id", typeof(Guid));
        return tabla;
    }

    private async Task<int> VolcarAsync(SqlConnection conexion, DataTable tabla, CancellationToken ct)
    {
        int cantidad = tabla.Rows.Count;

        using var copia = new SqlBulkCopy(conexion) { DestinationTableName = _definicion.Tabla };
        foreach (DataColumn columna in tabla.Columns)
            copia.ColumnMappings.Add(columna.ColumnName, columna.ColumnName);

        await copia.WriteToServerAsync(tabla, ct);
        tabla.Clear();
        return cantidad;
    }
}