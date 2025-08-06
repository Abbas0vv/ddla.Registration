using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAsset_DDLA.Migrations
{
    /// <inheritdoc />
    public partial class OneToMany_Relation_Fixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_StockProductId",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_StockProductId",
                table: "Products",
                column: "StockProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_StockProductId",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_StockProductId",
                table: "Products",
                column: "StockProductId",
                unique: true);
        }
    }
}
