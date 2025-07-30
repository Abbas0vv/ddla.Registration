using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAsset_DDLA.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryCodeInStockProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCount",
                table: "StockProducts");

            migrationBuilder.AddColumn<string>(
                name: "InventoryCode",
                table: "StockProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InventoryCode",
                table: "StockProducts");

            migrationBuilder.AddColumn<int>(
                name: "TotalCount",
                table: "StockProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
