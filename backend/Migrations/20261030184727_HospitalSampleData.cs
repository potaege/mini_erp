using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniErp.Api.Migrations
{
    /// <inheritdoc />
    public partial class HospitalSampleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "name", "password" },
                values: new object[,]
                {
                    { "demo.admin", "HospitalDemo1!" },
                    { "demo.stock1", "HospitalDemo2!" },
                    { "demo.stock2", "HospitalDemo3!" }
                });

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "name", "sku", "price", "stock_qty" },
                values: new object[,]
                {
                    { "หน้ากากอนามัยทางการแพทย์ 3 ชั้น", "HOSP-MASK-001", 5.50m, 500 },
                    { "ถุงมือไนไตรทางการแพทย์", "HOSP-GLOVE-001", 85.00m, 200 },
                    { "แอลกอฮอล์ 70% ขนาด 500 มล.", "HOSP-ALCOHOL-001", 45.00m, 120 },
                    { "ผ้าก๊อซปลอดเชื้อ 2 x 2 นิ้ว", "HOSP-GAUZE-001", 35.00m, 300 },
                    { "กระบอกฉีดยา 5 มล.", "HOSP-SYRINGE-001", 2.50m, 1000 },
                    { "เครื่องวัดอุณหภูมิแบบดิจิทัล", "HOSP-THERMO-001", 180.00m, 30 },
                    { "ผ้ายืดพันแผล", "HOSP-BANDAGE-001", 35.00m, 100 },
                    { "ชุดให้น้ำเกลือ", "HOSP-IV-SET-001", 45.00m, 80 },
                    { "น้ำเกลือปราศจากเชื้อ 0.9% ขนาด 1000 มล.", "HOSP-SALINE-001", 22.00m, 240 },
                    { "เสื้อคลุมป้องกันแบบใช้ครั้งเดียว", "HOSP-GOWN-001", 65.00m, 60 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM `product_logs`
                WHERE `user_id` IN (
                    SELECT `id` FROM `users`
                    WHERE `name` IN ('demo.admin', 'demo.stock1', 'demo.stock2')
                );
                """);

            migrationBuilder.Sql(
                """
                DELETE FROM `products`
                WHERE `sku` IN (
                    'HOSP-MASK-001',
                    'HOSP-GLOVE-001',
                    'HOSP-ALCOHOL-001',
                    'HOSP-GAUZE-001',
                    'HOSP-SYRINGE-001',
                    'HOSP-THERMO-001',
                    'HOSP-BANDAGE-001',
                    'HOSP-IV-SET-001',
                    'HOSP-SALINE-001',
                    'HOSP-GOWN-001'
                );
                """);

            migrationBuilder.Sql(
                """
                DELETE FROM `users`
                WHERE `name` IN ('demo.admin', 'demo.stock1', 'demo.stock2');
                """);
        }
    }
}
