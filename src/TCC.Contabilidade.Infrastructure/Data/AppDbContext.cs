using Microsoft.EntityFrameworkCore;
using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Nome)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(u => u.SenhaHash)
                .IsRequired();

            entity.Property(u => u.Perfil)
                .IsRequired()
                .HasMaxLength(50);
        });
    }
}