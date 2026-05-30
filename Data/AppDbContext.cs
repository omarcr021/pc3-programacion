using Microsoft.EntityFrameworkCore;
using TaskAnalysisAPI.Models;

namespace TaskAnalysisAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Tarea> Tareas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tarea>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Titulo)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(t => t.Descripcion)
                    .HasMaxLength(1000);

                entity.Property(t => t.Estado)
                    .IsRequired()
                    .HasConversion<string>();

                entity.Property(t => t.Prioridad)
                    .IsRequired()
                    .HasConversion<string>();

                entity.Property(t => t.FechaCreacion)
                    .IsRequired();
            });
        }
    }
}
