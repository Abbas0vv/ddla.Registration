using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAsset_DDLA.Migrations
{
    /// <inheritdoc />
    public partial class LocalUserStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReturned",
                table: "Transfers");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsReturned",
                table: "Transfers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
