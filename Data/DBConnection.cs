using GymManager.Models;
using GymManager.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

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


        public virtual DbSet<ReporteIngresoDTO> ReporteIngresos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. CONFIGURACIÓN DE LLAVES PRIMARIAS (IDENTITY)=
            // 2. CONFIGURACIÓN DE DETALLE DE VENTAS (Columna calculada)
            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.ToTable("DetalleVentas");
                entity.Property(d => d.subtotal)
                    .HasComputedColumnSql("[cantidad] * [precio_unitario]");
            });

            // 3. RELACIONES
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Usuario)
                .WithMany()
                .HasForeignKey(v => v.usuario_id);

            modelBuilder.Entity<Matricula>()
                .HasOne(m => m.Cliente)
                .WithMany()
                .HasForeignKey(m => m.cliente_id);

            // 4. CONFIGURACIÓN DE DTOs (Sin tabla física)
            modelBuilder.Entity<ReporteIngresoDTO>(entity => {
                entity.HasNoKey();
                entity.ToView(null);
            });
        }

        // --- PROCEDIMIENTOS ALMACENADOS ACTUALIZADOS ---

        public async Task<int> RegistrarClienteSP(string dni, string nombre, string apellido, string tel, string email)
        {
            return await Database.ExecuteSqlInterpolatedAsync(
                $"EXEC sp_RegistrarCliente @dni={dni}, @nombre={nombre}, @apellido={apellido}, @telefono={tel}, @email={email}");
        }
        public async Task<int> RegistrarVentaSP(long? clienteId, long usuarioId, decimal total)
        {
            return await Database.ExecuteSqlInterpolatedAsync(
                $"EXEC sp_RegistrarVenta @cliente_id={clienteId}, @usuario_id={usuarioId}, @total={total}");
        }
        public async Task<List<ReporteIngresoDTO>> ObtenerReporteIngresosSP(DateTime inicio, DateTime fin)
        {
            return await ReporteIngresos
                .FromSqlInterpolated($"EXEC sp_ReporteIngresos @fechaInicio={inicio.ToString("yyyy-MM-dd")}, @fechaFin={fin.ToString("yyyy-MM-dd")}")
                .ToListAsync();
        }
    }
}