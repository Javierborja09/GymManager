-- 1. CREACIÓN DE LA BASE DE DATOS
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'GymManagementDB')
BEGIN
    CREATE DATABASE GymManagementDB;
END
GO

USE GymManagementDB;
GO

-- 2. TABLAS

-- Usuarios del sistema
CREATE TABLE Usuarios (
    usuario_id BIGINT PRIMARY KEY IDENTITY(1,1), -- Autoincremental
    nombre VARCHAR(100) NOT NULL,
    email VARCHAR(150) UNIQUE NOT NULL,
    password_hash VARCHAR(MAX) NOT NULL,
    rol VARCHAR(20) CHECK (rol IN ('Admin', 'Vendedor')) DEFAULT 'Vendedor',
    fecha_creacion DATETIME DEFAULT GETDATE(),
    activo BIT DEFAULT 1
);

-- Clientes del gimnasio
CREATE TABLE Clientes (
    cliente_id BIGINT PRIMARY KEY IDENTITY(1,1), -- Autoincremental
    dni VARCHAR(20) UNIQUE NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    telefono VARCHAR(20),
    email VARCHAR(150),
    fecha_registro DATETIME DEFAULT GETDATE(),
    estado VARCHAR(20) CHECK (estado IN ('Activo', 'Inactivo', 'Deudor')) DEFAULT 'Activo'
);

-- Catálogo de Planes
CREATE TABLE Planes (
    plan_id BIGINT PRIMARY KEY IDENTITY(1,1),
    nombre_plan VARCHAR(50) NOT NULL,
    duracion_dias INT NOT NULL,
    precio DECIMAL(10,2) NOT NULL
);

-- Matrículas
CREATE TABLE Matriculas (
    matricula_id BIGINT PRIMARY KEY IDENTITY(1,1),
    cliente_id BIGINT NOT NULL,
    plan_id BIGINT NOT NULL,
    fecha_inicio DATE NOT NULL,
    fecha_fin DATE NOT NULL,
    monto_pagado DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (cliente_id) REFERENCES Clientes(cliente_id),
    FOREIGN KEY (plan_id) REFERENCES Planes(plan_id)
);

-- Productos
CREATE TABLE Productos (
    producto_id BIGINT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100) NOT NULL,
    precio_venta DECIMAL(10,2) NOT NULL,
    stock_actual INT NOT NULL DEFAULT 0,
    categoria VARCHAR(50)
);

-- Ventas (Cabecera)
CREATE TABLE Ventas (
    venta_id BIGINT PRIMARY KEY IDENTITY(1,1),
    cliente_id BIGINT NULL,
    usuario_id BIGINT NOT NULL,
    fecha_venta DATETIME DEFAULT GETDATE(),
    total_venta DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (cliente_id) REFERENCES Clientes(cliente_id),
    FOREIGN KEY (usuario_id) REFERENCES Usuarios(usuario_id)
);

-- Detalle de las ventas
CREATE TABLE DetalleVentas (
    detalle_id BIGINT PRIMARY KEY IDENTITY(1,1),
    venta_id BIGINT NOT NULL,
    producto_id BIGINT NOT NULL,
    cantidad INT NOT NULL,
    precio_unitario DECIMAL(10,2) NOT NULL,
    subtotal AS (cantidad * precio_unitario),
    FOREIGN KEY (venta_id) REFERENCES Ventas(venta_id),
    FOREIGN KEY (producto_id) REFERENCES Productos(producto_id)
);

-- Registro de Asistencia
CREATE TABLE Asistencias (
    asistencia_id BIGINT PRIMARY KEY IDENTITY(1,1),
    cliente_id BIGINT NOT NULL,
    fecha_hora DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (cliente_id) REFERENCES Clientes(cliente_id)
);
GO

-- 3. PROCEDIMIENTOS ALMACENADOS (Actualizados para omitir el ID en el INSERT)

-- Registrar un nuevo cliente
CREATE PROCEDURE sp_RegistrarCliente
    @dni VARCHAR(20),
    @nombre VARCHAR(100),
    @apellido VARCHAR(100),
    @telefono VARCHAR(20),
    @email VARCHAR(150)
AS
BEGIN
    INSERT INTO Clientes (dni, nombre, apellido, telefono, email)
    VALUES (@dni, @nombre, @apellido, @telefono, @email);
END;
GO

CREATE PROCEDURE sp_RegistrarVentaJson
    @cliente_id BIGINT = NULL,
    @usuario_id BIGINT,
    @total_venta DECIMAL(10,2),
    @productos_json NVARCHAR(MAX) -- Aquí recibimos la lista del carrito
AS
BEGIN
    BEGIN TRANSACTION
    BEGIN TRY
        -- 1. Insertar Cabecera
        DECLARE @nueva_venta_id BIGINT;
        INSERT INTO Ventas (cliente_id, usuario_id, total_venta, fecha_venta)
        VALUES (@cliente_id, @usuario_id, @total_venta, GETDATE());
        SET @nueva_venta_id = SCOPE_IDENTITY();

        -- 2. Insertar Detalles desde el JSON
        INSERT INTO DetalleVentas (venta_id, producto_id, cantidad, precio_unitario)
        SELECT 
            @nueva_venta_id,
            producto_id,
            cantidad,
            precio_unitario
        FROM OPENJSON(@productos_json)
        WITH (
            producto_id BIGINT,
            cantidad INT,
            precio_unitario DECIMAL(10,2)
        );

        -- 3. DESCONTAR STOCK masivamente
        UPDATE P
        SET P.stock_actual = P.stock_actual - J.cantidad
        FROM Productos P
        INNER JOIN OPENJSON(@productos_json)
        WITH (
            producto_id BIGINT,
            cantidad INT
        ) J ON P.producto_id = J.producto_id;

        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        THROW;
    END CATCH
END;
GO

CREATE PROCEDURE sp_ReporteIngresos
    @fechaInicio DATE,
    @fechaFin DATE
AS
BEGIN
    SELECT 'Membresías' as Tipo, SUM(monto_pagado) as Total
    FROM Matriculas WHERE fecha_inicio BETWEEN @fechaInicio AND @fechaFin
    UNION
    SELECT 'Venta Productos' as Tipo, SUM(total_venta) as Total
    FROM Ventas WHERE fecha_venta BETWEEN @fechaInicio AND @fechaFin;
END;
GO


-- Registrar o actualizar stock de producto
CREATE PROCEDURE sp_GuardarProducto
    @producto_id BIGINT = NULL,
    @nombre VARCHAR(100),
    @precio_venta DECIMAL(10,2),
    @stock_actual INT,
    @categoria VARCHAR(50)
AS
BEGIN
    IF @producto_id IS NULL OR @producto_id = 0
        INSERT INTO Productos (nombre, precio_venta, stock_actual, categoria)
        VALUES (@nombre, @precio_venta, @stock_actual, @categoria);
    ELSE
        UPDATE Productos 
        SET nombre = @nombre, precio_venta = @precio_venta, 
            stock_actual = @stock_actual, categoria = @categoria
        WHERE producto_id = @producto_id;
END;
GO


-- 1. PLANES (Membresías)
INSERT INTO Planes (nombre_plan, duracion_dias, precio) VALUES 
('Mensual Básico', 30, 80.00),
('Trimestral PRO', 90, 210.00),
('Anual Elite', 365, 750.00);

-- 2. USUARIOS (Personal del Gimnasio)
-- Nota: En un sistema real, los passwords deben ir hasheados. 
INSERT INTO Usuarios (nombre, email, password_hash, rol, activo) VALUES 
('Administrador General', 'admin@gym.com', 'AQAAAAIAAYagAAAAEJ9xGvWfH8pZ6kYmX4nL2tR5vS8wQ0zX1yB2c3D4E5F6G7H8I9J0K1L2M3N4O5P', 'Admin', 1),
('Vendedor Barranca', 'ventas@gym.com', 'AQAAAAIAAYagAAAAEJ9xGvWfH8pZ6kYmX4nL2tR5vS8wQ0zX1yB2c3D4E5F6G7H8I9J0K1L2M3N4O5P', 'Vendedor', 1)
select * from Usuarios
-- 3. CLIENTES (Socios)
INSERT INTO Clientes (dni, nombre, apellido, telefono, email, estado) VALUES 
('70654321', 'Carlos', 'Ruiz', '987654321', 'carlos.ruiz@gmail.com', 'Activo'),
('10203040', 'Ana', 'García', '912345678', 'ana.garcia@outlook.com', 'Activo'),
('40506070', 'Luis', 'Mendoza', '955444333', 'luis.m@yahoo.com', 'Deudor');

-- 4. PRODUCTOS (Venta en recepción)
INSERT INTO Productos (nombre, precio_venta, stock_actual, categoria) VALUES 
('Proteína Whey 1kg', 145.00, 12, 'Suplementos'),
('Creatina Monohidratada', 95.00, 8, 'Suplementos'),
('Agua Mineral 600ml', 2.50, 45, 'Bebidas'),
('Bebida Energética', 6.00, 20, 'Bebidas');

-- 5. MATRÍCULAS (Membresías activas)
INSERT INTO Matriculas (cliente_id, plan_id, fecha_inicio, fecha_fin, monto_pagado) VALUES 
(1, 1, GETDATE(), DATEADD(day, 30, GETDATE()), 80.00),
(2, 2, GETDATE(), DATEADD(day, 90, GETDATE()), 210.00);

-- 6. VENTAS (Ventas de productos)
INSERT INTO Ventas (cliente_id, usuario_id, total_venta) VALUES 
(1, 2, 147.50); -- Venta de Carlos Ruiz atendido por el Vendedor

-- 7. DETALLE DE VENTAS (El subtotal se autocalcula en tu SQL)
INSERT INTO DetalleVentas (venta_id, producto_id, cantidad, precio_unitario) VALUES 
(1, 1, 1, 145.00), -- 1 Proteína
(1, 3, 1, 2.50);   -- 1 Agua
GO



CREATE TABLE MetasMensuales (
    meta_id INT PRIMARY KEY IDENTITY(1,1),
    mes INT NOT NULL CHECK (mes BETWEEN 1 AND 12),
    anio INT NOT NULL,
    objetivo_monto DECIMAL(10,2) NOT NULL,
    descripcion VARCHAR(100)
);
GO

-- Cargar metas para el año 2026
INSERT INTO MetasMensuales (mes, anio, objetivo_monto, descripcion) VALUES 
(1, 2026, 5000.00, 'Enero: Inicio de año'),
(2, 2026, 4500.00, 'Febrero: Campaña verano'),
(3, 2026, 6000.00, 'Marzo: Retorno a clases/rutina'),
(4, 2026, 5500.00, 'Abril: Mantenimiento'),
(5, 2026, 5800.00, 'Mayo: Campaña Día de la Madre'),
(6, 2026, 5200.00, 'Junio: Mitad de año'),
(7, 2026, 7000.00, 'Julio: Gratificaciones/Fiestas Patrias'),
(8, 2026, 5500.00, 'Agosto: Fidelización'),
(9, 2026, 5800.00, 'Septiembre: Primavera'),
(10, 2026, 6200.00, 'Octubre: Aniversario Gym'),
(11, 2026, 6500.00, 'Noviembre: Pre-verano'),
(12, 2026, 8000.00, 'Diciembre: Campaña Navideña');
GO