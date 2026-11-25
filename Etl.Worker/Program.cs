using Etl.Aplicacion.Interfaces;
using Etl.Aplicacion.Servicios;
using Etl.Dominio.Entidades;
using Etl.Dominio.Interfaces;
using Etl.Infraestructura.Configuracion;
using Etl.Infraestructura.Extractores;
using Etl.Infraestructura.Staging;
using Etl.Worker;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<OpcionesCsv>(builder.Configuration.GetSection("FuenteCsv"));
builder.Services.PostConfigure<OpcionesCsv>(o =>
    o.RutaArchivo = Path.Combine(AppContext.BaseDirectory, o.RutaArchivo));
builder.Services.Configure<OpcionesBaseDatosOrigen>(builder.Configuration.GetSection("FuenteBaseDatos"));
builder.Services.Configure<OpcionesApi>(builder.Configuration.GetSection("FuenteApi"));
builder.Services.Configure<OpcionesStaging>(builder.Configuration.GetSection("Staging"));

builder.Services.AddHttpClient("ApiComentarios", (proveedor, cliente) =>
{
    var opciones = proveedor.GetRequiredService<IOptions<OpcionesApi>>().Value;
    cliente.BaseAddress = new Uri(opciones.UrlBase);
    cliente.DefaultRequestHeaders.Add("X-Api-Key", opciones.ClaveApi);
    cliente.Timeout = TimeSpan.FromSeconds(30);
});


builder.Services.AddTransient<IExtractor<EncuestaInterna>, CsvExtractor>();
builder.Services.AddTransient<IExtractor<ResenaWeb>, DatabaseExtractor>();
builder.Services.AddTransient<IExtractor<ComentarioSocial>, ApiExtractor>();

builder.Services.AddSingleton(DefinicionesStaging.Encuestas);
builder.Services.AddSingleton(DefinicionesStaging.Resenas);
builder.Services.AddSingleton(DefinicionesStaging.Comentarios);
builder.Services.AddTransient<IDataLoader<EncuestaInterna>, DataLoader<EncuestaInterna>>();
builder.Services.AddTransient<IDataLoader<ResenaWeb>, DataLoader<ResenaWeb>>();
builder.Services.AddTransient<IDataLoader<ComentarioSocial>, DataLoader<ComentarioSocial>>();
builder.Services.AddTransient<ICargadorDimensiones, CargadorDimensiones>();

builder.Services.AddTransient<OrquestadorExtraccion>();
builder.Services.AddHostedService<ServicioExtraccion>();



var host = builder.Build();
host.Run();