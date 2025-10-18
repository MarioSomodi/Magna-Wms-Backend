using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagnaWms.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitReferenceEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "wms");

            migrationBuilder.CreateTable(
                name: "Item",
                schema: "wms",
                columns: table => new
                {
                    ItemID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sku = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BaseUom = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    BaseUomFull = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    StandardCost = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    LeadTimeDays = table.Column<int>(type: "int", nullable: true),
                    ReorderPoint = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.ItemID);
                });

            migrationBuilder.CreateTable(
                name: "Warehouse",
                schema: "wms",
                columns: table => new
                {
                    WarehouseID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouse", x => x.WarehouseID);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                schema: "wms",
                columns: table => new
                {
                    LocationID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseID = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    MaxQty = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.LocationID);
                    table.CheckConstraint("CK_Location_Type", "Type IN ('RACK','BIN','FLOOR','STAGE')");
                    table.ForeignKey(
                        name: "FK_Location_Warehouse_WarehouseID",
                        column: x => x.WarehouseID,
                        principalSchema: "wms",
                        principalTable: "Warehouse",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Item_Sku",
                schema: "wms",
                table: "Item",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Location_WarehouseID_Code",
                schema: "wms",
                table: "Location",
                columns: new[] { "WarehouseID", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_Code",
                schema: "wms",
                table: "Warehouse",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Item",
                schema: "wms");

            migrationBuilder.DropTable(
                name: "Location",
                schema: "wms");

            migrationBuilder.DropTable(
                name: "Warehouse",
                schema: "wms");
        }
    }
}
