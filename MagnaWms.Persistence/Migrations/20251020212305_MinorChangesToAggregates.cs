using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagnaWms.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MinorChangesToAggregates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_Warehouse_WarehouseID",
                schema: "wms",
                table: "Location");

            migrationBuilder.RenameColumn(
                name: "WarehouseID",
                schema: "wms",
                table: "Location",
                newName: "WarehouseId");

            migrationBuilder.RenameIndex(
                name: "IX_Location_WarehouseID_Code",
                schema: "wms",
                table: "Location",
                newName: "IX_Location_WarehouseId_Code");

            migrationBuilder.AddForeignKey(
                name: "FK_Location_Warehouse_WarehouseId",
                schema: "wms",
                table: "Location",
                column: "WarehouseId",
                principalSchema: "wms",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_Warehouse_WarehouseId",
                schema: "wms",
                table: "Location");

            migrationBuilder.RenameColumn(
                name: "WarehouseId",
                schema: "wms",
                table: "Location",
                newName: "WarehouseID");

            migrationBuilder.RenameIndex(
                name: "IX_Location_WarehouseId_Code",
                schema: "wms",
                table: "Location",
                newName: "IX_Location_WarehouseID_Code");

            migrationBuilder.AddForeignKey(
                name: "FK_Location_Warehouse_WarehouseID",
                schema: "wms",
                table: "Location",
                column: "WarehouseID",
                principalSchema: "wms",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
