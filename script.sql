-- ============================================================
-- GymManagementDB - Script de instalación completo
-- Ejecutar en SQL Server Management Studio o sqlcmd
-- ============================================================

-- 1. CREAR BASE DE DATOS
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'GymManagementDB')
BEGIN
    CREATE DATABASE GymManagementDB;
END;
GO

USE GymManagementDB;
GO


-- ============================================================
-- 2. TABLAS
-- ============================================================

-- Usuarios del sistema
CREATE TABLE Usuarios (
    usuario_id    BIGINT PRIMARY KEY IDENTITY(1,1),
    nombre        VARCHAR(100) NOT NULL,
    email         VARCHAR(150) UNIQUE NOT NULL,
    password_hash VARCHAR(MAX) NOT NULL,
    rol           VARCHAR(20) CHECK (rol IN ('Admin', 'Vendedor')) DEFAULT 'Vendedor',
    fecha_creacion DATETIME DEFAULT GETDATE(),
    activo        BIT DEFAULT 1
);
GO

-- Clientes del gimnasio
CREATE TABLE Clientes (
    cliente_id     BIGINT PRIMARY KEY IDENTITY(1,1),
    dni            VARCHAR(20) UNIQUE NOT NULL,
    nombre         VARCHAR(100) NOT NULL,
    apellido       VARCHAR(100) NOT NULL,
    telefono       VARCHAR(20),
    email          VARCHAR(150),
    fecha_registro DATETIME DEFAULT GETDATE(),
    estado         VARCHAR(20) CHECK (estado IN ('Activo', 'Inactivo', 'Deudor')) DEFAULT 'Activo'
);
GO

-- Catálogo de Planes
CREATE TABLE Planes (
    plan_id       BIGINT PRIMARY KEY IDENTITY(1,1),
    nombre_plan   VARCHAR(50) NOT NULL,
    duracion_dias INT NOT NULL,
    precio        DECIMAL(10,2) NOT NULL
);
GO

-- Matrículas
CREATE TABLE Matriculas (
    matricula_id BIGINT PRIMARY KEY IDENTITY(1,1),
    cliente_id   BIGINT NOT NULL,
    plan_id      BIGINT NOT NULL,
    fecha_inicio DATE NOT NULL,
    fecha_fin    DATE NOT NULL,
    monto_pagado DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (cliente_id) REFERENCES Clientes(cliente_id),
    FOREIGN KEY (plan_id)    REFERENCES Planes(plan_id)
);
GO

-- Productos
CREATE TABLE Productos (
    producto_id  BIGINT PRIMARY KEY IDENTITY(1,1),
    nombre       VARCHAR(100) NOT NULL,
    precio_venta DECIMAL(10,2) NOT NULL,
    stock_actual INT NOT NULL DEFAULT 0,
    categoria    VARCHAR(50)
);
GO

-- Ventas (Cabecera)
CREATE TABLE Ventas (
    venta_id    BIGINT PRIMARY KEY IDENTITY(1,1),
    cliente_id  BIGINT NULL,
    usuario_id  BIGINT NOT NULL,
    fecha_venta DATETIME DEFAULT GETDATE(),
    total_venta DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (cliente_id) REFERENCES Clientes(cliente_id),
    FOREIGN KEY (usuario_id) REFERENCES Usuarios(usuario_id)
);
GO

-- Detalle de las ventas
CREATE TABLE DetalleVentas (
    detalle_id     BIGINT PRIMARY KEY IDENTITY(1,1),
    venta_id       BIGINT NOT NULL,
    producto_id    BIGINT NOT NULL,
    cantidad       INT NOT NULL,
    precio_unitario DECIMAL(10,2) NOT NULL,
    subtotal       AS (cantidad * precio_unitario),
    FOREIGN KEY (venta_id)    REFERENCES Ventas(venta_id),
    FOREIGN KEY (producto_id) REFERENCES Productos(producto_id)
);
GO

-- Registro de Asistencia
CREATE TABLE Asistencias (
    asistencia_id BIGINT PRIMARY KEY IDENTITY(1,1),
    cliente_id    BIGINT NOT NULL,
    fecha_hora    DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (cliente_id) REFERENCES Clientes(cliente_id)
);
GO

-- Metas Mensuales
CREATE TABLE MetasMensuales (
    meta_id        INT PRIMARY KEY IDENTITY(1,1),
    mes            INT NOT NULL CHECK (mes BETWEEN 1 AND 12),
    anio           INT NOT NULL,
    objetivo_monto DECIMAL(10,2) NOT NULL,
    descripcion    VARCHAR(100)
);
GO

-- ============================================================
-- 3. DATOS INICIALES (SEED)
-- ============================================================

-- Planes
INSERT INTO Planes (nombre_plan, duracion_dias, precio) VALUES
('Mensual Básico',  30,  80.00),
('Trimestral PRO',  90,  210.00),
('Anual Elite',     365, 750.00);
GO

-- Usuarios (passwords ya hasheados)
INSERT INTO Usuarios (nombre, email, password_hash, rol, activo) VALUES
('Administrador General', 'admin@gym.com',  'AQAAAAIAAYagAAAAEF5RRA/nu+ppe+mmdh7HF7g3hO/vX1dmE7GRRoU0BU3Qf8oC2+3W5NX/960GHkxpig==', 'Admin',    1),
('Vendedor Barranca',     'ventas@gym.com', 'AQAAAAIAAYagAAAAEF5RRA/nu+ppe+mmdh7HF7g3hO/vX1dmE7GRRoU0BU3Qf8oC2+3W5NX/960GHkxpig==', 'Vendedor', 1);
GO

-- Clientes
INSERT INTO Clientes (dni, nombre, apellido, telefono, email, estado) VALUES
('70654321', 'Carlos', 'Ruiz',    '987654321', 'carlos.ruiz@gmail.com',   'Activo'),
('10203040', 'Ana',    'García',  '912345678', 'ana.garcia@outlook.com',  'Activo'),
('40506070', 'Luis',   'Mendoza', '955444333', 'luis.m@yahoo.com',        'Deudor');
GO

-- Productos
INSERT INTO Productos (nombre, precio_venta, stock_actual, categoria) VALUES
('Proteína Whey 1kg',      145.00, 12, 'Suplementos'),
('Creatina Monohidratada',  95.00,  8, 'Suplementos'),
('Agua Mineral 600ml',       2.50, 45, 'Bebidas'),
('Bebida Energética',        6.00, 20, 'Bebidas');
GO

-- Matrículas
INSERT INTO Matriculas (cliente_id, plan_id, fecha_inicio, fecha_fin, monto_pagado) VALUES
(1, 1, GETDATE(), DATEADD(day, 30,  GETDATE()), 80.00),
(2, 2, GETDATE(), DATEADD(day, 90,  GETDATE()), 210.00);
GO

-- Ventas
INSERT INTO Ventas (cliente_id, usuario_id, total_venta) VALUES
(1, 2, 147.50);
GO

-- Detalle de Ventas
INSERT INTO DetalleVentas (venta_id, producto_id, cantidad, precio_unitario) VALUES
(1, 1, 1, 145.00),
(1, 3, 1,   2.50);
GO

-- Metas Mensuales 2026
INSERT INTO MetasMensuales (mes, anio, objetivo_monto, descripcion) VALUES
(1,  2026, 5000.00, 'Enero: Inicio de año'),
(2,  2026, 4500.00, 'Febrero: Campaña verano'),
(3,  2026, 6000.00, 'Marzo: Retorno a clases/rutina'),
(4,  2026, 5500.00, 'Abril: Mantenimiento'),
(5,  2026, 5800.00, 'Mayo: Campaña Día de la Madre'),
(6,  2026, 5200.00, 'Junio: Mitad de año'),
(7,  2026, 7000.00, 'Julio: Gratificaciones/Fiestas Patrias'),
(8,  2026, 5500.00, 'Agosto: Fidelización'),
(9,  2026, 5800.00, 'Septiembre: Primavera'),
(10, 2026, 6200.00, 'Octubre: Aniversario Gym'),
(11, 2026, 6500.00, 'Noviembre: Pre-verano'),
(12, 2026, 8000.00, 'Diciembre: Campaña Navideña');
GO

-- ============================================================
-- 4. STORED PROCEDURES — CLIENTES
-- ============================================================

CREATE PROCEDURE sp_RegistrarCliente
    @dni      VARCHAR(20),
    @nombre   VARCHAR(100),
    @apellido VARCHAR(100),
    @telefono VARCHAR(20),
    @email    VARCHAR(150)
AS
BEGIN
    INSERT INTO Clientes (dni, nombre, apellido, telefono, email)
    VALUES (@dni, @nombre, @apellido, @telefono, @email);
END;
GO

CREATE PROCEDURE sp_ListarClientes
    @Dni VARCHAR(20) = NULL
AS
BEGIN
    SELECT c.cliente_id, c.dni, c.nombre, c.apellido,
           c.telefono, c.email, c.estado
    FROM Clientes c
    WHERE (@Dni IS NULL OR dni LIKE '%' + @Dni + '%');
END;
GO

-- ============================================================
-- 5. STORED PROCEDURES — USUARIOS
-- ============================================================

CREATE PROCEDURE sp_ObtenerUsuarioPorEmail
    @Email NVARCHAR(150)
AS
BEGIN
    SELECT usuario_id, nombre, email, password_hash,
           rol, fecha_creacion, activo
    FROM Usuarios
    WHERE email = @Email;
END;
GO

-- ============================================================
-- 6. STORED PROCEDURES — PRODUCTOS
-- ============================================================

CREATE pROCEDURE sp_GuardarProducto
    @producto_id  BIGINT = NULL,
    @nombre       VARCHAR(100),
    @precio_venta DECIMAL(10,2),
    @stock_actual INT,
    @categoria    VARCHAR(50)
AS
BEGIN
    IF @producto_id IS NULL OR @producto_id = 0
        INSERT INTO Productos (nombre, precio_venta, stock_actual, categoria)
        VALUES (@nombre, @precio_venta, @stock_actual, @categoria);
    ELSE
        UPDATE Productos
        SET nombre       = @nombre,
            precio_venta = @precio_venta,
            stock_actual = @stock_actual,
            categoria    = @categoria
        WHERE producto_id = @producto_id;
END;
GO

CREATE PROCEDURE sp_ListarProductos
    @buscar VARCHAR(100) = NULL
AS
BEGIN
    SELECT producto_id, nombre, precio_venta, stock_actual, categoria
    FROM Productos
    WHERE (@buscar IS NULL
           OR nombre    LIKE '%' + @buscar + '%'
           OR categoria LIKE '%' + @buscar + '%');
END;
GO

CREATE  PROCEDURE sp_ObtenerProductoPorId
    @producto_id BIGINT
AS
BEGIN
    SELECT producto_id, nombre, precio_venta, stock_actual, categoria
    FROM Productos
    WHERE producto_id = @producto_id;
END;
GO

CREATE PROCEDURE sp_EliminarProducto
    @producto_id BIGINT
AS
BEGIN
    DELETE FROM Productos WHERE producto_id = @producto_id;
END;
GO

CREATE PROCEDURE sp_ListarProductosConStock
AS
BEGIN
    SELECT producto_id, nombre, precio_venta, stock_actual, categoria
    FROM Productos
    WHERE stock_actual > 0
    ORDER BY nombre;
END;
GO

-- ============================================================
-- 7. STORED PROCEDURES — PLANES
-- ============================================================

CREATE  PROCEDURE sp_ListarPlanes
AS
BEGIN
    SELECT plan_id, nombre_plan, duracion_dias, precio
    FROM Planes
    ORDER BY precio;
END;
GO

CREATE PROCEDURE sp_ObtenerPlanPorId
    @plan_id BIGINT
AS
BEGIN
    SELECT plan_id, nombre_plan, duracion_dias, precio
    FROM Planes
    WHERE plan_id = @plan_id;
END;
GO

CREATE PROCEDURE sp_InsertarPlan
    @nombre_plan   VARCHAR(50),
    @duracion_dias INT,
    @precio        DECIMAL(10,2)
AS
BEGIN
    INSERT INTO Planes (nombre_plan, duracion_dias, precio)
    VALUES (@nombre_plan, @duracion_dias, @precio);
    SELECT SCOPE_IDENTITY() AS plan_id;
END;
GO

CREATE  PROCEDURE sp_ActualizarPlan
    @plan_id       BIGINT,
    @nombre_plan   VARCHAR(50),
    @duracion_dias INT,
    @precio        DECIMAL(10,2)
AS
BEGIN
    UPDATE Planes
    SET nombre_plan   = @nombre_plan,
        duracion_dias = @duracion_dias,
        precio        = @precio
    WHERE plan_id = @plan_id;
END;
GO

-- ============================================================
-- 8. STORED PROCEDURES — MATRÍCULAS
-- ============================================================

CREATE PROCEDURE sp_ListarMatriculas
AS
BEGIN
    SELECT m.matricula_id,
           c.nombre, c.apellido, c.dni,
           p.nombre_plan,
           m.fecha_inicio, m.fecha_fin, m.monto_pagado
    FROM Matriculas m
    INNER JOIN Clientes c ON m.cliente_id = c.cliente_id
    INNER JOIN Planes   p ON m.plan_id    = p.plan_id
    ORDER BY m.fecha_inicio DESC;
END;
GO

CREATE PROCEDURE sp_ObtenerMatriculaPorId
    @matricula_id BIGINT
AS
BEGIN
    SELECT m.matricula_id, m.cliente_id, m.plan_id,
           m.fecha_inicio, m.fecha_fin, m.monto_pagado,
           c.nombre, c.apellido, c.dni, c.telefono,
           p.nombre_plan
    FROM Matriculas m
    INNER JOIN Clientes c ON m.cliente_id = c.cliente_id
    INNER JOIN Planes   p ON m.plan_id    = p.plan_id
    WHERE m.matricula_id = @matricula_id;
END;
GO

CREATE PROCEDURE sp_InsertarMatricula
    @cliente_id  BIGINT,
    @plan_id     BIGINT,
    @fecha_inicio DATE,
    @fecha_fin    DATE,
    @monto_pagado DECIMAL(10,2)
AS
BEGIN
    INSERT INTO Matriculas (cliente_id, plan_id, fecha_inicio, fecha_fin, monto_pagado)
    VALUES (@cliente_id, @plan_id, @fecha_inicio, @fecha_fin, @monto_pagado);
END;
GO

CREATE PROCEDURE sp_ObtenerTotalMatriculasMensual
    @mes  INT,
    @anio INT
AS
BEGIN
    SELECT ISNULL(SUM(monto_pagado), 0)
    FROM Matriculas
    WHERE MONTH(fecha_inicio) = @mes
      AND YEAR(fecha_inicio)  = @anio;
END;
GO

-- ============================================================
-- 9. STORED PROCEDURES — VENTAS
-- ============================================================

CREATE PROCEDURE sp_RegistrarVentaJson
    @cliente_id   BIGINT = NULL,
    @usuario_id   BIGINT,
    @total_venta  DECIMAL(10,2),
    @productos_json NVARCHAR(MAX)
AS
BEGIN
    BEGIN TRANSACTION
    BEGIN TRY
        DECLARE @nueva_venta_id BIGINT;
        INSERT INTO Ventas (cliente_id, usuario_id, total_venta, fecha_venta)
        VALUES (@cliente_id, @usuario_id, @total_venta, GETDATE());
        SET @nueva_venta_id = SCOPE_IDENTITY();

        INSERT INTO DetalleVentas (venta_id, producto_id, cantidad, precio_unitario)
        SELECT @nueva_venta_id, producto_id, cantidad, precio_unitario
        FROM OPENJSON(@productos_json)
        WITH (
            producto_id     BIGINT,
            cantidad        INT,
            precio_unitario DECIMAL(10,2)
        );

        UPDATE P
        SET P.stock_actual = P.stock_actual - J.cantidad
        FROM Productos P
        INNER JOIN OPENJSON(@productos_json)
        WITH (producto_id BIGINT, cantidad INT) J
            ON P.producto_id = J.producto_id;

        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        THROW;
    END CATCH
END;
GO

CREATE PROCEDURE sp_ObtenerTotalVentasMensual
    @mes  INT,
    @anio INT
AS
BEGIN
    SELECT ISNULL(SUM(total_venta), 0)
    FROM Ventas
    WHERE MONTH(fecha_venta) = @mes
      AND YEAR(fecha_venta)  = @anio;
END;
GO

CREATE PROCEDURE sp_ObtenerRecaudacionVentasPorFecha
    @fecha DATE
AS
BEGIN
    SELECT ISNULL(SUM(total_venta), 0)
    FROM Ventas
    WHERE CAST(fecha_venta AS DATE) = @fecha;
END;
GO

CREATE PROCEDURE sp_ReporteIngresos
    @fechaInicio DATE,
    @fechaFin    DATE
AS
BEGIN
    SELECT 'Membresías'       AS Tipo, SUM(monto_pagado) AS Total
    FROM Matriculas
    WHERE fecha_inicio BETWEEN @fechaInicio AND @fechaFin
    UNION
    SELECT 'Venta Productos'  AS Tipo, SUM(total_venta)  AS Total
    FROM Ventas
    WHERE fecha_venta  BETWEEN @fechaInicio AND @fechaFin;
END;
GO

-- ============================================================
-- 10. STORED PROCEDURES — METAS MENSUALES
-- ============================================================

CREATE PROCEDURE sp_ListarMetasMensuales
AS
BEGIN
    SELECT meta_id, mes, anio, objetivo_monto, descripcion
    FROM MetasMensuales
    ORDER BY mes;
END;
GO

CREATE PROCEDURE sp_ObtenerMetaMensualPorId
    @meta_id INT
AS
BEGIN
    SELECT meta_id, mes, anio, objetivo_monto, descripcion
    FROM MetasMensuales
    WHERE meta_id = @meta_id;
END;
GO

CREATE PROCEDURE sp_ActualizarMetaMensual
    @meta_id        INT,
    @objetivo_monto DECIMAL(10,2),
    @descripcion    VARCHAR(100)
AS
BEGIN
    UPDATE MetasMensuales
    SET objetivo_monto = @objetivo_monto,
        descripcion    = @descripcion
    WHERE meta_id = @meta_id;
END;
GO


-- Obtener por ID
CREATE PROCEDURE sp_ObtenerClientePorId 
    @ClienteId BIGINT
AS
BEGIN
    SELECT cliente_id, dni, nombre, apellido, telefono, email, fecha_registro, estado
    FROM Clientes 
    WHERE cliente_id = @ClienteId;
END;
GO

-- 3. Actualizar
CREATE PROCEDURE sp_ActualizarCliente
    @ClienteId BIGINT, 
    @Dni NVARCHAR(20), 
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100), 
    @Telefono NVARCHAR(50),
    @Email NVARCHAR(150), 
    @Estado NVARCHAR(50)
AS
BEGIN
    UPDATE Clientes 
    SET dni=@Dni, 
        nombre=@Nombre, 
        apellido=@Apellido,
        telefono=@Telefono, 
        email=@Email, 
        estado=@Estado
    WHERE cliente_id = @ClienteId;
END;
GO
-- ============================================================
-- FIN DEL SCRIPT
-- ============================================================