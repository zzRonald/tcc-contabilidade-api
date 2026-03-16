using Microsoft.EntityFrameworkCore;
using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Usuarios { get; set; }

    public DbSet<Convite> Convites { get; set; }

    // 🔹 ADICIONAR ISSO
    public DbSet<Empresa> Empresas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
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

            entity.Property(u => u.TipoUsuario)
                .IsRequired();
        });

        // 🔹 CONFIGURAÇÃO DA EMPRESA
        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Nome)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(e => e.CNPJ)
                .IsRequired()
                .HasMaxLength(14);

            entity.HasIndex(e => e.CNPJ)
                .IsUnique();

            entity.HasOne(e => e.Cliente)
                .WithMany()
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}