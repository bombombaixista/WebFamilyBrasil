using Microsoft.EntityFrameworkCore;
using Kanban.Models;

public class AppDbContext : DbContext
{
    public DbSet<Cliente2> Clientes2 { get; set; }
    public DbSet<Plano> Planos { get; set; }
    public DbSet<Cobranca> Cobrancas { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<ItemPedido> ItensPedido { get; set; }
    public DbSet<LogIntegracao> LogIntegracoes { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cliente2
        modelBuilder.Entity<Cliente2>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Nome)
                  .HasMaxLength(150)
                  .IsRequired();

            entity.Property(c => c.Email)
                  .HasMaxLength(150)
                  .IsRequired();

            entity.Property(c => c.SenhaHash)
                  .HasMaxLength(255)
                  .IsRequired();

            entity.Property(c => c.DataCadastro)
                  .HasColumnType("datetime");

            entity.Property(c => c.Ativo)
                  .HasDefaultValue(true);

            entity.HasOne(c => c.Plano)
                  .WithMany()
                  .HasForeignKey(c => c.PlanoId)
                  .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Pedido>()
    .HasOne(p => p.Cliente2)
    .WithMany()
    .HasForeignKey(p => p.Cliente2Id)
    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemPedido>()
                .HasOne(i => i.Pedido)
                .WithMany(p => p.Itens)
                .HasForeignKey(i => i.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pedido>()
                .HasIndex(p => p.MarketplaceOrderId);
        });

        // Plano
        modelBuilder.Entity<Plano>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Codigo)
                  .HasMaxLength(5)
                  .IsRequired();

            entity.Property(p => p.Nome)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(p => p.PrecoMensal)
                  .HasColumnType("decimal(10,2)");
        });

        // Cobranca
        modelBuilder.Entity<Cobranca>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Valor)
                  .HasColumnType("decimal(10,2)");

            entity.Property(c => c.DataCobranca)
                  .HasColumnType("datetime");

            entity.Property(c => c.Status)
                  .HasMaxLength(50)
                  .HasDefaultValue("Pendente");

            entity.HasOne(c => c.Cliente)
                  .WithMany()
                  .HasForeignKey(c => c.ClienteId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seeding dos planos
        modelBuilder.Entity<Plano>().HasData(
            new Plano { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Codigo = "C", Nome = "Básico", PrecoMensal = 79.90M },
            new Plano { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Codigo = "B", Nome = "Profissional", PrecoMensal = 99.90M },
            new Plano { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Codigo = "A", Nome = "Empresarial", PrecoMensal = 119.90M }
        );
    }
}
