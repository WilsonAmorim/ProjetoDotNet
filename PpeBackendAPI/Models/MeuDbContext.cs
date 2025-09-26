using Microsoft.EntityFrameworkCore;

namespace PpeBackendAPI.Models
{
    public class MeuDbContext : DbContext
    {
        public MeuDbContext(DbContextOptions<MeuDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}
