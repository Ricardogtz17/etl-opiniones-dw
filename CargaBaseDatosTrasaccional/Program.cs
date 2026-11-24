using CargaBaseDatosTrasaccional.Extraccion;
using CargaBaseDatosTrasaccional.Validacion;
using Microsoft.Extensions.Configuration;
using CargaBaseDatosTrasaccional.AccesoDatos;
using CargaBaseDatosTrasaccional.Servicios;
using CargaBaseDatosTrasaccional.Entidates;

namespace CargaBaseDatosTrasaccional
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            using var db = new EjecutorProcedimientos(config.GetConnectionString("AnalisisOpiniones")!);
            var procesador = new ProcesadorTabla(new LectorCsv(), db, config["RutaDatos"]!);

            var idsClientes = new HashSet<string>();
            var clientes = new DefinicionTabla<Cliente>
            {
                Nombre = "Clientes",
                Archivo = "clients.csv",
                ColumnasEsperadas = new[] { "IdCliente", "Nombre", "Email" },
                IdsCargados = idsClientes,
                Validar = fila => ValidadorCliente.Validar(fila, idsClientes),
                ObtenerId = c => c.IdCliente,
                Procedimiento = "usp_InsertarCliente",
                ObtenerParametros = c => new (string, object?)[]
                {
                    ("@IdCliente", c.IdCliente), ("@Nombre",  c.Nombre), ("@Email", c.Email)
                }
            };

            var idsProductos = new HashSet<string> ();
            var productos = new DefinicionTabla<Producto>
            {
                Nombre = "Productos",
                Archivo = "products.csv",
                ColumnasEsperadas = new[] {"IdProducto", "Nombre" , "Categoria"},
                IdsCargados= idsProductos,
                Validar = fila => ValidadorProducto.Validar (fila, idsProductos),
                ObtenerId = p => p.IdProducto,
                Procedimiento = "usp_InsertarProducto",
                ObtenerParametros = p => new (string, object?)[]
                {
                    ("@IdProducto",  p.IdProducto), ("@Nombre", p.Nombre), ("@Categoria",  p.Categoria), 
                }
            };

            var idsFuentes = new HashSet<string> ();
            var fuentes = new DefinicionTabla<FuenteDatos>
            {
                Nombre = "FuenteDatos",
                Archivo = "fuente_datos.csv",
                ColumnasEsperadas = new[] { "IdFuente", "TipoFuente", "FechaCarga" },
                IdsCargados = idsFuentes,
                Validar = fila => ValidadorFuenteDatos.Validar(fila, idsFuentes),
                ObtenerId = f => f.IdFuente,
                Procedimiento = "usp_InsertarFuenteDatos",
                ObtenerParametros = f => new (string, object?)[]
                {
                    ("@IdFuente",  f.IdFuente), ("@TipoFuente", f.TipoFuente), ("FechaCarga",  f.FechaCarga),
                }
            };

            var idsReviews = new HashSet<string>();
            var reviews = new DefinicionTabla<WebReview>
            {
                Nombre = "Web_Reviews",
                Archivo = "web_reviews.csv",
                ColumnasEsperadas = new[] { "IdReview", "IdCliente", "IdProducto", "Fecha", "Comentario", "Rating" },
                IdsCargados = idsReviews,
                Validar = fila => ValidadorWebReview.Validar(fila, idsReviews, idsClientes, idsProductos),
                ObtenerId = w => w.IdReview,
                Procedimiento = "usp_InsertarWebReview",
                ObtenerParametros = w => new (string, object?)[]
                {
                    ("@IdReview", w.IdReview), ("@IdCliente", w.IdCliente), ("@IdProducto", w.IdProducto),
                    ("@Fecha", w.Fecha), ("@Comentario", w.Comentario), ("@Rating", w.Rating)
                }

            };

            var idsSurveys = new HashSet<string>();
            var surveys = new DefinicionTabla<SurveyPart1>
            {
                Nombre = "Surveys_Part1",
                Archivo = "surveys_part1.csv",
                ColumnasEsperadas = new[] { "IdOpinion", "IdCliente", "IdProducto", "Fecha", "Comentario",
                                          "Clasificacion", "PuntajeSatisfaccion", "Fuente" },
                IdsCargados = idsSurveys,
                Validar = fila =>ValidadorSurveyPart1.Validar(fila,idsSurveys, idsClientes,idsProductos),
                ObtenerId = s => s.IdOpinion,
                Procedimiento = "usp_InsertarSurveyPart1",
                ObtenerParametros = s => new (string, object?)[]
                {
                     ("@IdOpinion", s.IdOpinion), ("@IdCliente", s.IdCliente), ("@IdProducto", s.IdProducto),
                     ("@Fecha", s.Fecha), ("@Comentario", s.Comentario), ("@Clasificacion", s.Clasificacion),
                     ("@PuntajeSatisfaccion", s.PuntajeSatisfaccion), ("@Fuente", s.Fuente)
                }
            };

            var idsSocial  = new HashSet<string>();
            var social = new DefinicionTabla<SocialComment>
            {
                Nombre = "Social_Comments",
                Archivo = "Social_comments.csv",
                ColumnasEsperadas = new[] { "IdComment", "IdCliente", "IdProducto", "Fuente", "Fecha", "Comentario" },
                IdsCargados = idsSocial,
                Validar = fila => ValidadorSocialComment.Validar(fila, idsSocial, idsClientes, idsProductos),
                ObtenerId = s => s.IdComment,
                Procedimiento = "usp_InsertarSocialComment",
                ObtenerParametros = s => new (string, object?)[]
                {
                    ("@IdComment", s.IdComment), ("@IdCliente", s.IdCliente), ("@IdProducto", s.IdProducto),
                    ("@Fuente", s.Fuente), ("@Fecha", s.Fecha), ("@Comentario", s.Comentario)
                }
            };

            var resultados = new List<ResultadoTabla>
            {
                  procesador.Procesar(clientes),
                  procesador.Procesar(productos),
                  procesador.Procesar(fuentes),
                  procesador.Procesar(reviews),
                  procesador.Procesar(surveys),
                  procesador.Procesar(social)
            };

            ReporteFinal.Imprimir(resultados, Path.Combine(AppContext.BaseDirectory, "rechazos.txt"));
        }
    }

}
