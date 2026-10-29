using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniErp.Api.Migrations
{
    /// <inheritdoc />
    public partial class ProductChangeTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_logs_product",
                table: "product_logs");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "product_logs",
                type: "enum('CREATE','NAME','SKU','PRICE','STOCK_IN','STOCK_OUT','DELETE')",
                nullable: false,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "enum('CREATE','NAME','PRICE','STOCK_IN','STOCK_OUT','DELETE')")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "change_qty",
                table: "product_logs",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "change_qty",
                table: "product_logs");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "product_logs",
                type: "enum('CREATE','NAME','PRICE','STOCK_IN','STOCK_OUT','DELETE')",
                nullable: false,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "enum('CREATE','NAME','SKU','PRICE','STOCK_IN','STOCK_OUT','DELETE')")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.AddForeignKey(
                name: "fk_logs_product",
                table: "product_logs",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id");
        }
    }
}
