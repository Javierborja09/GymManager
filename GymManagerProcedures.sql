USE GymManagementDB;
GO

-- ============================================================
-- PRODUCTOS
-- ============================================================
-- Listar Productos con filtro
CREATE OR ALTER PROCEDURE sp_ListarProductos
    @buscar VARCHAR(100) = NULL
AS
BEGIN
    SELECT producto_id, nombre, precio_venta, stock_actual, categoria
    FROM Productos
    WHERE (@buscar IS NULL 
           OR nombre LIKE '%' + @buscar + '%' 
           OR categoria LIKE '%' + @buscar + '%')
END
GO

-- Obtener por ID
CREATE OR ALTER PROCEDURE sp_ObtenerProductoPorId
    @producto_id BIGINT
AS
BEGIN
    SELECT producto_id, nombre, precio_venta, stock_actual, categoria
    FROM Productos
    WHERE producto_id = @producto_id;
END
GO

-- Eliminar Producto
CREATE OR ALTER PROCEDURE sp_EliminarProducto
    @producto_id BIGINT
AS
BEGIN
    DELETE FROM Productos WHERE producto_id = @producto_id;
END
GO
--Lista de productos con stock 
CREATE OR ALTER PROCEDURE sp_ListarProductosConStock
AS
BEGIN
    SELECT producto_id, nombre, precio_venta, stock_actual, categoria
    FROM Productos
    WHERE stock_actual > 0
    ORDER BY nombre;
END
GO

-- ============================================================
-- PLANES
-- ============================================================
-- Listar Plan
CREATE OR ALTER PROCEDURE sp_ListarPlanes
AS
BEGIN
    SELECT plan_id, nombre_plan, duracion_dias, precio
    FROM Planes
    ORDER BY precio;
END
GO

-- Obtener por ID
CREATE OR ALTER PROCEDURE sp_ObtenerPlanPorId
    @plan_id BIGINT
AS
BEGIN
    SELECT plan_id, nombre_plan, duracion_dias, precio
    FROM Planes
    WHERE plan_id = @plan_id;
END
GO

-- Insertar Plan 
CREATE OR ALTER PROCEDURE sp_InsertarPlan
    @nombre_plan   VARCHAR(50),
    @duracion_dias INT,
    @precio        DECIMAL(10,2)
AS
BEGIN
    INSERT INTO Planes (nombre_plan, duracion_dias, precio)
    VALUES (@nombre_plan, @duracion_dias, @precio);
 
    SELECT SCOPE_IDENTITY() AS plan_id;
END
GO

-- Actualizar Plan 
CREATE OR ALTER PROCEDURE sp_ActualizarPlan
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
END
GO

-- ============================================================
-- METAS MENSUALES
-- ============================================================
-- Obetener Metas mensaules 
CREATE OR ALTER PROCEDURE sp_ListarMetasMensuales
AS
BEGIN
    SELECT meta_id, mes, anio, objetivo_monto, descripcion
    FROM MetasMensuales
    ORDER BY mes;
END
GO


-- Obetener Metas mensaules por ID
CREATE OR ALTER PROCEDURE sp_ObtenerMetaMensualPorId
    @meta_id INT
AS
BEGIN
    SELECT meta_id, mes, anio, objetivo_monto, descripcion
    FROM MetasMensuales
    WHERE meta_id = @meta_id;
END
GO

-- Actualizar metas mensuales
CREATE OR ALTER PROCEDURE sp_ActualizarMetaMensual
    @meta_id       INT,
    @objetivo_monto DECIMAL(10,2),
    @descripcion    VARCHAR(100)
AS
BEGIN
    UPDATE MetasMensuales
    SET objetivo_monto = @objetivo_monto,
        descripcion    = @descripcion
    WHERE meta_id = @meta_id;
END
GO

-- ============================================================
-- Matricula
-- ============================================================
-- 1. Listar Matrículas 
CREATE OR ALTER PROCEDURE sp_ListarMatriculas
AS
BEGIN
    SELECT 
        m.matricula_id,
        c.nombre,
        c.apellido, 
        c.dni,
        p.nombre_plan,
        m.fecha_inicio,
        m.fecha_fin,
        m.monto_pagado
    FROM Matriculas m
    INNER JOIN Clientes c ON m.cliente_id = c.cliente_id
    INNER JOIN Planes p ON m.plan_id = p.plan_id
    ORDER BY m.fecha_inicio DESC;
END
GO


-- 2. Obtener Matrículas por id
CREATE OR ALTER PROCEDURE sp_ObtenerMatriculaPorId
    @matricula_id BIGINT
AS
BEGIN
    SELECT 
        m.matricula_id, m.cliente_id, m.plan_id, m.fecha_inicio, m.fecha_fin, m.monto_pagado,
        c.nombre, c.apellido, c.dni, c.telefono,
        p.nombre_plan
    FROM Matriculas m
    INNER JOIN Clientes c ON m.cliente_id = c.cliente_id
    INNER JOIN Planes p ON m.plan_id = p.plan_id
    WHERE m.matricula_id = @matricula_id;
END
GO

-- 3. Insertar Nueva Matrícula
CREATE OR ALTER PROCEDURE sp_InsertarMatricula
    @cliente_id BIGINT,
    @plan_id BIGINT,
    @fecha_inicio DATE,
    @fecha_fin DATE,
    @monto_pagado DECIMAL(10,2)
AS
BEGIN
    INSERT INTO Matriculas (cliente_id, plan_id, fecha_inicio, fecha_fin, monto_pagado)
    VALUES (@cliente_id, @plan_id, @fecha_inicio, @fecha_fin, @monto_pagado);

END
GO


-- Total recaudado por matrículas en un mes/año
CREATE OR ALTER PROCEDURE sp_ObtenerTotalMatriculasMensual
    @mes INT,
    @anio INT
AS
BEGIN
    SELECT ISNULL(SUM(monto_pagado), 0) 
    FROM Matriculas 
    WHERE MONTH(fecha_inicio) = @mes AND YEAR(fecha_inicio) = @anio;
END
GO

-- ============================================================
-- Ventas
-- ============================================================

-- Total recaudado por ventas en un mes/año
CREATE OR ALTER PROCEDURE sp_ObtenerTotalVentasMensual
    @mes INT,
    @anio INT
AS
BEGIN
    SELECT ISNULL(SUM(total_venta), 0) 
    FROM Ventas 
    WHERE MONTH(fecha_venta) = @mes AND YEAR(fecha_venta) = @anio;
END
GO

-- Obtener recaudación de ventas de un día específico 
CREATE OR ALTER PROCEDURE sp_ObtenerRecaudacionVentasPorFecha
    @fecha DATE
AS
BEGIN
    SELECT ISNULL(SUM(total_venta), 0)
    FROM Ventas
    WHERE CAST(fecha_venta AS DATE) = @fecha;
END
GO
-- ============================================================
-- Clientes
-- ============================================================
CREATE OR ALTER PROCEDURE sp_ListarClientes
 @Dni VARCHAR(20) = NULL
AS
BEGIN
    SELECT 
       c.cliente_id,
       c.dni,
       c.nombre,
       c.apellido,
       c.telefono,
       c.email,
       c.estado
    FROM Clientes c
         WHERE (@Dni IS NULL 
           OR dni LIKE '%' + @Dni + '%')
END
GO
