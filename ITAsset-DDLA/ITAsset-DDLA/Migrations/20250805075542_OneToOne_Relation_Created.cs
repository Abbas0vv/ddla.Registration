using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAsset_DDLA.Migrations
{
    /// <inheritdoc />
    public partial class OneToOne_Relation_Created : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_StockProductId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "InUseCount",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_StockProductId",
                table: "Products",
                column: "StockProductId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_StockProductId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "InUseCount",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_StockProductId",
                table: "Products",
                column: "StockProductId");
        }
    }
}
