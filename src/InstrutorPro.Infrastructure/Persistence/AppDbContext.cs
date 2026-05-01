using InstrutorPro.Domain.Entities;
using InstrutorPro.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace InstrutorPro.Infrastructure.Persistence;

/// <summary>
/// DbContext principal da aplicação.
/// Configura as entidades e seus mapeamentos para o PostgreSQL.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Instrutor> Instrutores => Set<Instrutor>();
    public DbSet<Aluno> Alunos => Set<Aluno>();
    public DbSet<Agendamento> Agendamentos => Set<Agendamento>();
    public DbSet<Avaliacao> Avaliacoes => Set<Avaliacao>();
    public DbSet<Assinatura> Assinaturas => Set<Assinatura>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Instrutor
        modelBuilder.Entity<Instrutor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CPF).IsRequired().HasMaxLength(11);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Telefone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(2);
            entity.Property(e => e.ValorHoraAula).HasPrecision(10, 2);
            entity.HasIndex(e => e.CPF).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();

            // Armazena a lista de categorias de habilitação como JSON no PostgreSQL
            entity.Property(e => e.CategoriasHabilitacao)
                .HasConversion(
                    v => string.Join(',', v.Select(c => c.ToString())),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(c => Enum.Parse<CategoriaHabilitacao>(c))
                          .ToList()
                );

            // Value Object Credenciamento como owned entity (colunas embutidas)
            entity.OwnsOne(e => e.Credenciamento, credenciamento =>
            {
                credenciamento.Property(c => c.NumeroCredencial).HasMaxLength(50);
            });
        });

        // Configuração da entidade Aluno
        modelBuilder.Entity<Aluno>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CPF).IsRequired().HasMaxLength(11);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Telefone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(2);
            entity.HasIndex(e => e.CPF).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configuração da entidade Agendamento
        modelBuilder.Entity<Agendamento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Valor).HasPrecision(10, 2);

            entity.HasOne(e => e.Instrutor)
                  .WithMany(i => i.Agendamentos)
                  .HasForeignKey(e => e.InstrutorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Aluno)
                  .WithMany(a => a.Agendamentos)
                  .HasForeignKey(e => e.AlunoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração da entidade Avaliacao
        modelBuilder.Entity<Avaliacao>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Agendamento)
                  .WithOne(a => a.Avaliacao)
                  .HasForeignKey<Avaliacao>(e => e.AgendamentoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração da entidade Assinatura
        modelBuilder.Entity<Assinatura>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PlanoId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.AsaasSubscriptionId).HasMaxLength(100);

            entity.HasOne(e => e.Instrutor)
                  .WithOne(i => i.Assinatura)
                  .HasForeignKey<Assinatura>(e => e.InstrutorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
