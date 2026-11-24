using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var claveEsperada = app.Configuration["ClaveApi"]
    ?? throw new InvalidOperationException("Falta la configuración 'ClaveApi' (user-secrets).");

var rutaCsv = Path.Combine(AppContext.BaseDirectory, "Datos", "social_comments.csv");
var configuracionCsv = new CsvConfiguration(CultureInfo.InvariantCulture)
{
    PrepareHeaderForMatch = args => args.Header.Trim().ToLowerInvariant(),
    MissingFieldFound = null,
    HeaderValidated = null
};

List<ComentarioDto> comentarios;
using (var lector = new StreamReader(rutaCsv))
using (var csv = new CsvReader(lector, configuracionCsv))
{
    comentarios = csv.GetRecords<ComentarioDto>().ToList();
}

app.MapGet("/salud", () => "API simulada en línea");

app.MapGet("/api/comentarios", (HttpContext http, int pagina = 1, int tamanoPagina = 50) =>
{
    var clave = http.Request.Headers["X-Api-Key"].ToString();
    if (clave != claveEsperada)
        return Results.Unauthorized();

    pagina = Math.Max(pagina, 1);
    tamanoPagina = Math.Clamp(tamanoPagina, 1, 200);

    var datos = comentarios.Skip((pagina - 1) * tamanoPagina).Take(tamanoPagina).ToList();
    return Results.Ok(new { pagina, tamanoPagina, total = comentarios.Count, datos });
});

app.Run();

public class ComentarioDto
{
    public string IdComment { get; set; } = string.Empty;
    public string? IdCliente { get; set; }
    public string? IdProducto { get; set; }
    public string? Fuente { get; set; }
    public DateTime? Fecha { get; set; }
    public string? Comentario { get; set; }
}