using Microsoft.EntityFrameworkCore;
using devsu.Models;

namespace devsu.Data
{
    public class DevsuContext : DbContext
    {
        public DevsuContext(DbContextOptions<DevsuContext> options) : base(options) { }
        
        public DbSet<Cliente> Clientes { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>()
                .HasBaseType<Persona>();
            
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Identificacion)
                .IsUnique();
        }
    }
}