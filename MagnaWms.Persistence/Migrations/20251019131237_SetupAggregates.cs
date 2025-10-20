using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagnaWms.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SetupAggregates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseUom",
                schema: "wms",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "BaseUomFull",
                schema: "wms",
                table: "Item");

            migrationBuilder.RenameColumn(
                name: "WarehouseID",
                schema: "wms",
                table: "Warehouse",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "LocationID",
                schema: "wms",
                table: "Location",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ItemID",
                schema: "wms",
                table: "Item",
                newName: "Id");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "wms",
                table: "Location",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "wms",
                table: "Item",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<long>(
                name: "UnitOfMeasureId",
                schema: "wms",
                table: "Item",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "UnitOfMeasure",
                schema: "wms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitOfMeasure", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Item_UnitOfMeasureId",
                schema: "wms",
                table: "Item",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasure_Symbol",
                schema: "wms",
                table: "UnitOfMeasure",
                column: "Symbol",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Item_UnitOfMeasure_UnitOfMeasureId",
                schema: "wms",
                table: "Item",
                column: "UnitOfMeasureId",
                principalSchema: "wms",
                principalTable: "UnitOfMeasure",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_UnitOfMeasure_UnitOfMeasureId",
                schema: "wms",
                table: "Item");

            migrationBuilder.DropTable(
                name: "UnitOfMeasure",
                schema: "wms");

            migrationBuilder.DropIndex(
                name: "IX_Item_UnitOfMeasureId",
                schema: "wms",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "wms",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "wms",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasureId",
                schema: "wms",
                table: "Item");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "wms",
                table: "Warehouse",
                newName: "WarehouseID");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "wms",
                table: "Location",
                newName: "LocationID");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "wms",
                table: "Item",
                newName: "ItemID");

            migrationBuilder.AddColumn<string>(
                name: "BaseUom",
                schema: "wms",
                table: "Item",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BaseUomFull",
                schema: "wms",
                table: "Item",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }
    }
}
