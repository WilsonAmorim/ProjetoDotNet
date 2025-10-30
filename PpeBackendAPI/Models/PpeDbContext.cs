using Microsoft.EntityFrameworkCore;

namespace PpeBackendAPI.Models
{
    public class PpeDbContext : DbContext
    {
        public PpeDbContext(DbContextOptions<PpeDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Tarefa> Tarefas { get; set; }
        public DbSet<Convenio> Convenios { get; set; }
        public DbSet<Conferencias> Conferencias { get; set; }
        public DbSet<Documentos> Documentos { get; set; }
        public DbSet<Ocorrencias> Ocorrencias { get; set; }
        public DbSet<RegistroOcorrencias> RegistroOcorrencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tarefa>()
                .Property(t => t.status)
                .HasConversion<string>();
        }


    }
}
