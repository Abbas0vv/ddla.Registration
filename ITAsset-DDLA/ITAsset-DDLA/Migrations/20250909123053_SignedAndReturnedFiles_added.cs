using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAsset_DDLA.Migrations
{
    /// <inheritdoc />
    public partial class SignedAndReturnedFiles_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Transfers");

            migrationBuilder.AddColumn<string>(
                name: "ReturnedFilePath",
                table: "StockProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignedFilePath",
                table: "StockProducts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnedFilePath",
                table: "StockProducts");

            migrationBuilder.DropColumn(
                name: "SignedFilePath",
                table: "StockProducts");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Transfers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
