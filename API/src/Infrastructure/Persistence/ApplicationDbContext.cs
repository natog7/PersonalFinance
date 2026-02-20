using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceAPI.Domain.Entities;
using PersonalFinanceAPI.Domain.Enums;

namespace PersonalFinanceAPI.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core DbContext for the Personal Finance API application.
/// Configures all domain entities and their relationships.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

	public DbSet<User> Users { get; set; } = null!;
	public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUser(modelBuilder);
        ConfigureTransaction(modelBuilder);
        ConfigureRecurrentTransaction(modelBuilder);
        ConfigureCategory(modelBuilder);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<User>();

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256)
            .IsUnicode(false);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Nickname)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.Role)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<UserRole>(v))
            .IsRequired();

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }

    private static void ConfigureTransaction(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Transaction>();

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(t => t.Type)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<TransactionType>(v))
            .IsRequired();

        builder.Property(t => t.Date)
            .IsRequired()
			.HasDefaultValueSql("CURRENT_TIMESTAMP");

		builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(t => t.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Money value object
        builder.OwnsOne(t => t.Amount, amountBuilder =>
        {
            amountBuilder.Property(m => m.Amount)
                .HasColumnName("Amount")
                .IsRequired();

            amountBuilder.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Relationships
        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indices
        builder.HasIndex(t => t.Type);
        builder.HasIndex(t => t.Date);
        builder.HasIndex(t => t.CategoryId);
        builder.HasIndex(t => t.CreatedAt);
    }

    private static void ConfigureRecurrentTransaction(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<RecurrentTransaction>();

        builder.HasBaseType<Transaction>();

        builder.Property(rt => rt.Period)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<RecurrentPeriod>(v))
            .IsRequired();

		builder.Property(t => t.EndDate)
			.IsRequired();

        // Indices
        builder.HasIndex(t => t.Period);
	}

	private static void ConfigureCategory(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Category>();

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(c => c.Description)
            .HasMaxLength(512);

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(c => c.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Self-referencing relationship for parent-child categories
        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Index for hierarchical queries
        builder.HasIndex(c => c.ParentCategoryId);
    }
}
