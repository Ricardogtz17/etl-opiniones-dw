use opiniones_dw;
go

/*Columna email en dim_cliente */
IF COL_LENGTH('dbo.dim_cliente', 'email') IS NULL
    ALTER TABLE dbo.dim_cliente ADD email varchar(100) NULL;
GO

/* dim_fuente (3 valores fijos) */
CREATE OR ALTER PROCEDURE dbo.usp_CargarDimFuente
AS
BEGIN
    SET NOCOUNT ON;

    MERGE dbo.dim_fuente AS t
    USING (VALUES
        ('Encuesta interna', 'CSV',           'Interno'),
        ('Reseña web',       'BD relacional', 'Sitio Web'),
        ('Red Social',       'API REST',      'Redes Sociales')
    ) AS s (nombre_fuente, tipo_origen, canal)
        ON t.nombre_fuente = s.nombre_fuente AND t.canal = s.canal
    WHEN MATCHED AND t.tipo_origen <> s.tipo_origen THEN
        UPDATE SET t.tipo_origen = s.tipo_origen
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (nombre_fuente, tipo_origen, canal)
        VALUES (s.nombre_fuente, s.tipo_origen, s.canal);
END
GO

/*dim_sentimiento (3 valores fijos) */
CREATE OR ALTER PROCEDURE dbo.usp_CargarDimSentimiento
AS
BEGIN
    SET NOCOUNT ON;

    MERGE dbo.dim_sentimiento AS t
    USING (VALUES
        ('Positivo', 'Opinion favorable'),
        ('Negativo', 'Opinion desfavorable'),
        ('Neutro',   'Opinion sin carga emocional clara')
    ) AS s (etiqueta, descripcion)
        ON t.etiqueta = s.etiqueta
    WHEN MATCHED AND ISNULL(t.descripcion, '') <> s.descripcion THEN
        UPDATE SET t.descripcion = s.descripcion
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (etiqueta, descripcion)
        VALUES (s.etiqueta, s.descripcion);
END
GO

/* dim_sentimiento (3 valores fijos) */
CREATE OR ALTER PROCEDURE dbo.usp_CargarDimSentimiento
AS
BEGIN
    SET NOCOUNT ON;

    MERGE dbo.dim_sentimiento AS t
    USING (VALUES
        ('Positivo', 'Opinion favorable'),
        ('Negativo', 'Opinion desfavorable'),
        ('Neutro',   'Opinion sin carga emocional clara')
    ) AS s (etiqueta, descripcion)
        ON t.etiqueta = s.etiqueta
    WHEN MATCHED AND ISNULL(t.descripcion, '') <> s.descripcion THEN
        UPDATE SET t.descripcion = s.descripcion
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (etiqueta, descripcion)
        VALUES (s.etiqueta, s.descripcion);
END
GO

/* dim_producto <- AnalisisOpiniones.dbo.Productos */
CREATE OR ALTER PROCEDURE dbo.usp_CargarDimProducto
AS
BEGIN
    SET NOCOUNT ON;

    MERGE dbo.dim_producto AS t
    USING (
        SELECT CAST(IdProducto AS varchar(30)) AS codigo_producto,
               Nombre,
               Categoria
        FROM AnalisisOpiniones.dbo.Productos
    ) AS s
        ON t.codigo_producto = s.codigo_producto
    WHEN MATCHED AND (   t.nombre <> s.Nombre
                      OR ISNULL(t.categoria, '') <> ISNULL(s.Categoria, '')) THEN
        UPDATE SET t.nombre    = s.Nombre,
                   t.categoria = s.Categoria
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (codigo_producto, nombre, categoria)
        VALUES (s.codigo_producto, s.Nombre, s.Categoria);
END
GO

/*dim_tiempo (se genera por calendario) */
CREATE OR ALTER PROCEDURE dbo.usp_CargarDimTiempo
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @min date, @max date;

    SELECT @min = MIN(f), @max = MAX(f)
    FROM (
        SELECT Fecha AS f FROM AnalisisOpiniones.dbo.Web_Reviews
        UNION ALL
        SELECT Fecha      FROM AnalisisOpiniones.dbo.Surveys_Part1
        UNION ALL
        SELECT Fecha      FROM AnalisisOpiniones.dbo.Social_Comments
    ) AS x
    WHERE f IS NOT NULL;

    IF @min IS NULL RETURN;

    SET @min = DATEFROMPARTS(YEAR(@min), 1, 1);
    SET @max = DATEFROMPARTS(YEAR(@max), 12, 31);

    ;WITH fechas AS (
        SELECT @min AS d
        UNION ALL
        SELECT DATEADD(DAY, 1, d) FROM fechas WHERE d < @max
    )
    INSERT INTO dbo.dim_tiempo (id_tiempo, fecha, dia, semana, mes, nombre_mes, trimestre, anio)
    SELECT CONVERT(int, CONVERT(char(8), d, 112)),
           d,
           CAST(DAY(d) AS tinyint),
           CAST(DATEPART(ISO_WEEK, d) AS tinyint),
           CAST(MONTH(d) AS tinyint),
           CHOOSE(MONTH(d), 'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
                            'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'),
           CAST(DATEPART(QUARTER, d) AS tinyint),
           CAST(YEAR(d) AS smallint)
    FROM fechas AS f
    WHERE NOT EXISTS (SELECT 1 FROM dbo.dim_tiempo AS t WHERE t.fecha = f.d)
    OPTION (MAXRECURSION 0);
END
GO

/*Carga dim_clientes */
CREATE OR ALTER PROCEDURE dbo.usp_CargarDimCliente
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.dim_cliente WHERE codigo_cliente = 'ANONIMO')
        INSERT INTO dbo.dim_cliente (codigo_cliente, nombre, pais, tipo_cliente)
        VALUES ('ANONIMO', 'Cliente anonimo', 'Desconocido', 'Desconocido');

    MERGE dbo.dim_cliente AS t
    USING (
        SELECT CAST(IdCliente AS varchar(40)) AS codigo_cliente,
               Nombre,
               Email
        FROM AnalisisOpiniones.dbo.Clientes
    ) AS s
        ON t.codigo_cliente = s.codigo_cliente
    WHEN MATCHED AND (   ISNULL(t.nombre, '') <> ISNULL(s.Nombre, '')
                      OR ISNULL(t.email,  '') <> ISNULL(s.Email,  '')) THEN
        UPDATE SET t.nombre = s.Nombre,
                   t.email  = s.Email
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (codigo_cliente, nombre, email, tipo_cliente)
        VALUES (s.codigo_cliente, s.Nombre, s.Email, 'Registrado');
END
GO

/*Procedimiento maestro */

/*Procedimiento maestro */
CREATE OR ALTER PROCEDURE dbo.usp_CargarDimensiones
AS
BEGIN
    SET NOCOUNT ON;

    EXEC dbo.usp_CargarDimFuente;
    EXEC dbo.usp_CargarDimSentimiento;
    EXEC dbo.usp_CargarDimCliente;
    EXEC dbo.usp_CargarDimProducto;
    EXEC dbo.usp_CargarDimTiempo;

    SELECT 'dim_fuente'      AS tabla, COUNT(*) AS filas FROM dbo.dim_fuente
    UNION ALL SELECT 'dim_sentimiento', COUNT(*) FROM dbo.dim_sentimiento
    UNION ALL SELECT 'dim_cliente',     COUNT(*) FROM dbo.dim_cliente
    UNION ALL SELECT 'dim_producto',    COUNT(*) FROM dbo.dim_producto
    UNION ALL SELECT 'dim_tiempo',      COUNT(*) FROM dbo.dim_tiempo;
END
GO

