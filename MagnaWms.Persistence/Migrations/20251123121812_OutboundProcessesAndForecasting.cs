using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagnaWms.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OutboundProcessesAndForecasting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ForecastSeries",
                schema: "wms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    HorizonDays = table.Column<int>(type: "int", nullable: false),
                    ModelInfo = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastSeries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PickTask",
                schema: "wms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    SalesOrderId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    CompletedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    CompletedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrder",
                schema: "wms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shipment",
                schema: "wms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    SalesOrderId = table.Column<long>(type: "bigint", nullable: false),
                    ShipmentNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Carrier = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TrackingNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ShippedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    ShippedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForecastSeriesPoint",
                schema: "wms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ForecastSeriesId = table.Column<long>(type: "bigint", nullable: false),
                    ForecastDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastSeriesPoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastSeriesPoint_ForecastSeries_ForecastSeriesId",
                        column: x => x.ForecastSeriesId,
                        principalSchema: "wms",
                        principalTable: "ForecastSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PickTaskLine",
                schema: "wms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickTaskId = table.Column<long>(type: "bigint", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    LocationId = table.Column<long>(type: "bigint", nullable: false),
                    QuantityToPick = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    QuantityPicked = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickTaskLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickTaskLine_PickTask_PickTaskId",
                        column: x => x.PickTaskId,
                        principalSchema: "wms",
                        principalTable: "PickTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderLine",
                schema: "wms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesOrderId = table.Column<long>(type: "bigint", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    QuantityOrdered = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    QuantityAllocated = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    QuantityPicked = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOrderLine_SalesOrder_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalSchema: "wms",
                        principalTable: "SalesOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentLine",
                schema: "wms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentId = table.Column<long>(type: "bigint", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    QuantityShipped = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentLine_Shipment_ShipmentId",
                        column: x => x.ShipmentId,
                        principalSchema: "wms",
                        principalTable: "Shipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ForecastSeries_WarehouseId_ItemId_CreatedUtc",
                schema: "wms",
                table: "ForecastSeries",
                columns: new[] { "WarehouseId", "ItemId", "CreatedUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ForecastSeriesPoint_ForecastSeriesId_ForecastDateUtc",
                schema: "wms",
                table: "ForecastSeriesPoint",
                columns: new[] { "ForecastSeriesId", "ForecastDateUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PickTaskLine_PickTaskId",
                schema: "wms",
                table: "PickTaskLine",
                column: "PickTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLine_SalesOrderId",
                schema: "wms",
                table: "SalesOrderLine",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentLine_ShipmentId",
                schema: "wms",
                table: "ShipmentLine",
                column: "ShipmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForecastSeriesPoint",
                schema: "wms");

            migrationBuilder.DropTable(
                name: "PickTaskLine",
                schema: "wms");

            migrationBuilder.DropTable(
                name: "SalesOrderLine",
                schema: "wms");

            migrationBuilder.DropTable(
                name: "ShipmentLine",
                schema: "wms");

            migrationBuilder.DropTable(
                name: "ForecastSeries",
                schema: "wms");

            migrationBuilder.DropTable(
                name: "PickTask",
                schema: "wms");

            migrationBuilder.DropTable(
                name: "SalesOrder",
                schema: "wms");

            migrationBuilder.DropTable(
                name: "Shipment",
                schema: "wms");
        }
    }
}
