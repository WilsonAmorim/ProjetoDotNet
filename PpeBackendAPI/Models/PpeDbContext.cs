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
        public DbSet<Repasse> Repasses { get; set; }
        public DbSet<Estagio> Estagios { get; set; }
        public DbSet<Investimento> Investimentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tarefa>()
                .Property(t => t.status)
                .HasConversion<string>();

            modelBuilder.Entity<Tarefa>()
                .Property(t => t.dataCriacao)
                .HasColumnType("datetime(6)");


            modelBuilder.Entity<Tarefa>()
                .Property(t => t.dataExecucao)
                .HasColumnType("datetime(6)");

            modelBuilder.Entity<Usuario>()
                .Property(t => t.RefreshTokenExpiracao)
                .HasColumnType("datetime(6)");

            modelBuilder.Entity<Conferencias>().Property(t => t.DataRetorno).HasColumnType("datetime(6)");
            modelBuilder.Entity<Conferencias>().Property(t => t.DataAtualizacao).HasColumnType("datetime(6)");

            modelBuilder.Entity<Convenio>().Property(t => t.DataAdmissao).HasColumnType("datetime(6)");
            modelBuilder.Entity<Convenio>().Property(t => t.DataDemissao).HasColumnType("datetime(6)");
            modelBuilder.Entity<Convenio>().Property(t => t.DataAtualizacao).HasColumnType("datetime(6)");

            modelBuilder.Entity<Repasse>().Property(t => t.DataPagamento).HasColumnType("datetime(6)");


        }


    }
}
