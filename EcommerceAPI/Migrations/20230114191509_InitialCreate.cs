using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_OrderData_OrderId",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "OrderDetails",
                newName: "OrderDataId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_OrderDataId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OrderData",
                newName: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_OrderData_OrderDataId",
                table: "OrderDetails",
                column: "OrderDataId",
                principalTable: "OrderData",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_OrderData_OrderDataId",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "OrderDataId",
                table: "OrderDetails",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_OrderDataId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_OrderId");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "OrderData",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_OrderData_OrderId",
                table: "OrderDetails",
                column: "OrderId",
                principalTable: "OrderData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
