using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAsset_DDLA.Migrations
{
    /// <inheritdoc />
    public partial class QuantityRemoved_GuidAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "StockProducts");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupCode",
                table: "StockProducts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GroupCode",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupCode",
                table: "StockProducts");

            migrationBuilder.DropColumn(
                name: "GroupCode",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "StockProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
