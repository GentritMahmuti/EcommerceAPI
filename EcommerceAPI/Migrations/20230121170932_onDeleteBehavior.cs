using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class onDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductOrderDatas_Products_ProductId",
                table: "ProductOrderDatas");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOrderDatas_Products_ProductId",
                table: "ProductOrderDatas",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductOrderDatas_Products_ProductId",
                table: "ProductOrderDatas");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOrderDatas_Products_ProductId",
                table: "ProductOrderDatas",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
