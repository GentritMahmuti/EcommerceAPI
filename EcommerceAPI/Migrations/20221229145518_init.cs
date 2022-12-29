using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CoverType_CoverTypeId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoverType",
                table: "CoverType");

            migrationBuilder.RenameTable(
                name: "CoverType",
                newName: "CoverTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoverTypes",
                table: "CoverTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CoverTypes_CoverTypeId",
                table: "Products",
                column: "CoverTypeId",
                principalTable: "CoverTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CoverTypes_CoverTypeId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoverTypes",
                table: "CoverTypes");

            migrationBuilder.RenameTable(
                name: "CoverTypes",
                newName: "CoverType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoverType",
                table: "CoverType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CoverType_CoverTypeId",
                table: "Products",
                column: "CoverTypeId",
                principalTable: "CoverType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
