using GymManager.Models;
using GymManager.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GymManager.Data
{
    public class DBConnection : DbContext
    {
        public DBConnection(DbContextOptions<DBConnection> options)
            : base(options)
        {
        }

        // --- TABLAS ---
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Plan> Planes { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }


        public DbSet<MetaMensual> MetasMensuales { get; set; }

        // --- DTOs PARA REPORTES ---
        public virtual DbSet<ReporteIngresoDTO> ReporteIngresos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. CONFIGURACIÓN DE DETALLE DE VENTAS (Columna calculada en SQL)
            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.HasOne<Venta>() 
                    .WithMany(v => v.DetalleVentas) 
                    .HasForeignKey(d => d.venta_id);
            });

            // 2. RELACIONES
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Usuario)
                .WithMany()
                .HasForeignKey(v => v.usuario_id);

            modelBuilder.Entity<Matricula>()
                .HasOne(m => m.Cliente)
                .WithMany()
                .HasForeignKey(m => m.cliente_id);

            // 3. CONFIGURACIÓN DE DTOs (Sin tabla física)
            modelBuilder.Entity<ReporteIngresoDTO>(entity => {
                entity.HasNoKey();
                entity.ToView(null);
            });
        }

        // --- LLAMADAS A PROCEDIMIENTOS ALMACENADOS NUEVOS ---

        public async Task<int> RegistrarClienteSP(string dni, string nombre, string apellido, string tel, string email)
        {
            return await Database.ExecuteSqlInterpolatedAsync(
                $"EXEC sp_RegistrarCliente @dni={dni}, @nombre={nombre}, @apellido={apellido}, @telefono={tel}, @email={email}");
        }

        // Gestión de Productos (Crea o Actualiza)
        public async Task<int> GuardarProductoSP(long? id, string nombre, decimal precio, int stock, string cat)
        {
            return await Database.ExecuteSqlInterpolatedAsync(
                $"EXEC sp_GuardarProducto @producto_id={id}, @nombre={nombre}, @precio_venta={precio}, @stock_actual={stock}, @categoria={cat}");
        }

        // Registro de Venta con Descuento de Stock Automático
        public async Task RegistrarVentaJsonSP(long? clienteId, long usuarioId, decimal total, string jsonProductos)
        {
            await Database.ExecuteSqlInterpolatedAsync(
                $"EXEC sp_RegistrarVentaJson @cliente_id={clienteId}, @usuario_id={usuarioId}, @total_venta={total}, @productos_json={jsonProductos}");
        }

        // Registro de Matrícula (Membresía)
        public async Task<int> RegistrarMatriculaSP(long clienteId, long planId, decimal monto)
        {
            return await Database.ExecuteSqlInterpolatedAsync(
                $"EXEC sp_RegistrarMatricula @cliente_id={clienteId}, @plan_id={planId}, @monto={monto}");
        }

        // Control de Asistencia por DNI
        public async Task<int> RegistrarAsistenciaSP(string dni)
        {
            return await Database.ExecuteSqlInterpolatedAsync(
                $"EXEC sp_RegistrarAsistencia @dni={dni}");
        }

        // Reportes de Ingresos
        public async Task<List<ReporteIngresoDTO>> ObtenerReporteIngresosSP(DateTime inicio, DateTime fin)
        {
            return await ReporteIngresos
                .FromSqlInterpolated($"EXEC sp_ReporteIngresos @fechaInicio={inicio:yyyy-MM-dd}, @fechaFin={fin:yyyy-MM-dd}")
                .ToListAsync();
        }
    }
}