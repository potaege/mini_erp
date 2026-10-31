using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniErp.Api.Migrations
{
    /// <inheritdoc />
    public partial class ProductNonNegativeConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "ck_products_price_nonnegative",
                table: "products",
                sql: "`price` >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_products_stock_nonnegative",
                table: "products",
                sql: "`stock_qty` >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_products_price_nonnegative",
                table: "products");

            migrationBuilder.DropCheckConstraint(
                name: "ck_products_stock_nonnegative",
                table: "products");
        }
    }
}
