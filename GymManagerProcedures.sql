USE GymManagementDB;
GO

-- ============================================================
-- PRODUCTOS
-- ============================================================

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
CREATE OR ALTER PROCEDURE sp_ProductoPorID
    @producto_id INT
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

