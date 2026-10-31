using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MiniErp.Api.Models;

namespace MiniErp.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<ProductLog> ProductLogs => Set<ProductLog>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasCharSet("utf8mb4").UseCollation("utf8mb4_general_ci");

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products", table =>
            {
                table.HasCheckConstraint("ck_products_price_nonnegative", "`price` >= 0");
                table.HasCheckConstraint("ck_products_stock_nonnegative", "`stock_qty` >= 0");
            });
            entity.HasKey(product => product.Id);
            entity.HasIndex(product => product.Sku)
                .IsUnique()
                .HasDatabaseName("uq_products_sku");
            entity.HasIndex(product => product.Name)
                .HasDatabaseName("ix_products_name");
            entity.Property(product => product.Id)
                .HasColumnName("id")
                .HasColumnType("int unsigned")
                .ValueGeneratedOnAdd();
            entity.Property(product => product.Name)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();
            entity.Property(product => product.Sku)
                .HasColumnName("sku")
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(product => product.Price)
                .HasColumnName("price")
                .HasPrecision(12, 2);
            entity.Property(product => product.StockQuantity)
                .HasColumnName("stock_qty")
                .HasDefaultValue(0);
            entity.Property(product => product.IsActive)
                .HasColumnName("is_active")
                .HasColumnType("tinyint(1)")
                .HasDefaultValue(true);
            entity.Property(product => product.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(product => product.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<ProductLog>(entity =>
        {
            entity.ToTable("product_logs");
            entity.HasKey(log => log.Id);
            entity.HasIndex(log => new { log.CreatedAt, log.Id })
                .HasDatabaseName("ix_logs_created_at");
            entity.HasIndex(log => new { log.ProductId, log.CreatedAt, log.Id })
                .HasDatabaseName("ix_logs_product");
            entity.HasIndex(log => log.UserId)
                .HasDatabaseName("ix_logs_user");
            entity.Property(log => log.Id)
                .HasColumnName("id")
                .HasColumnType("bigint unsigned")
                .ValueGeneratedOnAdd();
            entity.Property(log => log.ProductId)
                .HasColumnName("product_id")
                .HasColumnType("int unsigned");
            entity.Property(log => log.UserId)
                .HasColumnName("user_id")
                .HasColumnType("int unsigned");
            entity.Property(log => log.Type)
                .HasColumnName("type")
                .HasConversion(new ValueConverter<ProductLogType, string>(
                    value => ToDatabaseType(value),
                    value => FromDatabaseType(value)))
                .HasColumnType("enum('CREATE','NAME','SKU','PRICE','STOCK_IN','STOCK_OUT','DELETE')");
            entity.Property(log => log.OldValue)
                .HasColumnName("old_value")
                .HasMaxLength(255);
            entity.Property(log => log.NewValue)
                .HasColumnName("new_value")
                .HasMaxLength(255);
            entity.Property(log => log.ChangeQuantity)
                .HasColumnName("change_qty")
                .HasColumnType("int");
            entity.Property(log => log.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne(log => log.User)
                .WithMany(user => user.ProductLogs)
                .HasForeignKey(log => log.UserId)
                .HasConstraintName("fk_logs_user")
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(user => user.Id);
            entity.HasIndex(user => user.Name)
                .IsUnique()
                .HasDatabaseName("uq_users_name");
            entity.Property(user => user.Id)
                .HasColumnName("id")
                .HasColumnType("int unsigned")
                .ValueGeneratedOnAdd();
            entity.Property(user => user.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(user => user.Password)
                .HasColumnName("password")
                .HasMaxLength(30)
                .IsRequired();
            entity.Property(user => user.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }

    private static string ToDatabaseType(ProductLogType value)
    {
        return value switch
        {
            ProductLogType.Create => "CREATE",
            ProductLogType.Name => "NAME",
            ProductLogType.Sku => "SKU",
            ProductLogType.Price => "PRICE",
            ProductLogType.StockIn => "STOCK_IN",
            ProductLogType.StockOut => "STOCK_OUT",
            ProductLogType.Delete => "DELETE",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    private static ProductLogType FromDatabaseType(string value)
    {
        return value switch
        {
            "CREATE" => ProductLogType.Create,
            "NAME" => ProductLogType.Name,
            "SKU" => ProductLogType.Sku,
            "PRICE" => ProductLogType.Price,
            "STOCK_IN" => ProductLogType.StockIn,
            "STOCK_OUT" => ProductLogType.StockOut,
            "DELETE" => ProductLogType.Delete,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}
