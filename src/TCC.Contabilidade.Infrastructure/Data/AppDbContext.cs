using Microsoft.EntityFrameworkCore;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;
using TCC.Contabilidade.Domain.Interfaces;

namespace TCC.Contabilidade.Infrastructure.Data;

public class AppDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<User> Usuarios { get; set; }

    public DbSet<Convite> Convites { get; set; }

    public DbSet<Empresa> Empresas { get; set; }

    public DbSet<UsuarioEmpresa> UsuariosEmpresas { get; set; }

    public DbSet<CompanyConfig> CompanyConfigs { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<AuditLog> AuditLogs { get; set; }

    public DbSet<Funcionario> Funcionarios { get; set; }

    public DbSet<Notification> Notifications { get; set; }

    public DbSet<Competencia> Competencias { get; set; }

    public override int SaveChanges()
    {
        ApplyTenantId();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTenantId();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyTenantId()
    {
        foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (_tenantContext.TenantId.HasValue)
                    {
                        entry.Entity.EmpresaId = _tenantContext.TenantId.Value;
                    }
                    break;
                case EntityState.Modified:
                    // Impede que o EmpresaId seja alterado
                    entry.Property(e => e.EmpresaId).IsModified = false;
                    break;
            }
        }
    }

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

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);

            entity.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasOne(rt => rt.Usuario)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(rt => rt.Token)
                .IsUnique();
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Acao)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.Entidade)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.EntidadeId)
                .HasMaxLength(100);

            entity.Property(a => a.Ip)
                .HasMaxLength(50);

            entity.Property(a => a.DataHora)
                .IsRequired();
        });

        modelBuilder.Entity<Funcionario>(entity =>
        {
            entity.HasKey(f => f.Id);

            entity.Property(f => f.Nome)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(f => f.CPF)
                .IsRequired()
                .HasMaxLength(11);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.Id);

            entity.Property(n => n.EmailDestino)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(n => n.Mensagem)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(n => n.Tipo)
                .IsRequired();

            entity.Property(n => n.DataEnvio)
                .IsRequired();
        });

        modelBuilder.Entity<Competencia>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Mes).IsRequired();
            entity.Property(c => c.Ano).IsRequired();
            entity.Property(c => c.Status).IsRequired();
            entity.Property(c => c.DataCriacao).IsRequired();

            entity.Property(c => c.Observacoes)
                .HasMaxLength(500);

            entity.HasOne(c => c.Empresa)
                .WithMany()
                .HasForeignKey(c => c.EmpresaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(c => new { c.EmpresaId, c.Mes, c.Ano })
                .IsUnique();
        });

        // Global Query Filters for Multi-Tenancy
        modelBuilder.Entity<CompanyConfig>().HasQueryFilter(e => e.EmpresaId == _tenantContext.TenantId);
        modelBuilder.Entity<Funcionario>().HasQueryFilter(e => e.EmpresaId == _tenantContext.TenantId);
        modelBuilder.Entity<Notification>().HasQueryFilter(e => e.EmpresaId == _tenantContext.TenantId);
        modelBuilder.Entity<AuditLog>().HasQueryFilter(e => e.EmpresaId == _tenantContext.TenantId);
        modelBuilder.Entity<Competencia>().HasQueryFilter(e => e.EmpresaId == _tenantContext.TenantId);
    }
}
