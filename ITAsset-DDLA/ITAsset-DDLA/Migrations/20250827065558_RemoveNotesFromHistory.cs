using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAsset_DDLA.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNotesFromHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "TransferHistories");

            migrationBuilder.AddColumn<int>(
                name: "TransferStatus",
                table: "Transfers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransferStatus",
                table: "Transfers");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "TransferHistories",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
