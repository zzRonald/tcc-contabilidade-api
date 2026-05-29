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

    public DbSet<Empresa> Empresas { get; set; }

    public DbSet<UsuarioEmpresa> UsuariosEmpresas { get; set; }

    public DbSet<CompanyConfig> CompanyConfigs { get; set; }

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

            entity.HasOne(u => u.Contador)
                .WithMany(u => u.Clientes)
                .HasForeignKey(u => u.ContadorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

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
        });

        modelBuilder.Entity<UsuarioEmpresa>(entity =>
        {
            entity.HasKey(ue => ue.Id);

            entity.HasOne(ue => ue.Usuario)
                .WithMany(u => u.UsuariosEmpresas)
                .HasForeignKey(ue => ue.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ue => ue.Empresa)
                .WithMany(e => e.UsuariosEmpresas)
                .HasForeignKey(ue => ue.EmpresaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(ue => new { ue.UsuarioId, ue.EmpresaId }).IsUnique();
        });

        modelBuilder.Entity<CompanyConfig>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.MoedaPadrao)
                .HasMaxLength(10)
                .IsRequired();

            entity.Property(c => c.FormatoData)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(c => c.Timezone)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasOne(c => c.Empresa)
                .WithOne(e => e.Config)
                .HasForeignKey<CompanyConfig>(c => c.EmpresaId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
