using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniErp.Api.Migrations
{
    /// <inheritdoc />
    public partial class ProductLogQueryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_logs_product",
                table: "product_logs");

            migrationBuilder.CreateIndex(
                name: "ix_logs_created_at",
                table: "product_logs",
                columns: new[] { "created_at", "id" });

            migrationBuilder.CreateIndex(
                name: "ix_logs_product",
                table: "product_logs",
                columns: new[] { "product_id", "created_at", "id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_logs_created_at",
                table: "product_logs");

            migrationBuilder.DropIndex(
                name: "ix_logs_product",
                table: "product_logs");

            migrationBuilder.CreateIndex(
                name: "ix_logs_product",
                table: "product_logs",
                columns: new[] { "product_id", "created_at" });
        }
    }
}
