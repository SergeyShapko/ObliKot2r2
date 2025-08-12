using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ObliKot2r2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Philia",
                columns: table => new
                {
                    IdPhilia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NamePhilia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdPhiliaSAP = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IdAnother = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ObjPhilia = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Philia", x => x.IdPhilia);
                });

            migrationBuilder.CreateTable(
                name: "PointIOTypes",
                columns: table => new
                {
                    IdTypePointIO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameTypePointIO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointIOTypes", x => x.IdTypePointIO);
                });

            migrationBuilder.CreateTable(
                name: "Subdivisions",
                columns: table => new
                {
                    IdSubdivision = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPhilia = table.Column<int>(type: "int", nullable: false),
                    NameSubdivision = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IdSubdivisionSAP = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IdAnother = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ObjSubdivision = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subdivisions", x => x.IdSubdivision);
                    table.ForeignKey(
                        name: "FK_Subdivisions_Philia_IdPhilia",
                        column: x => x.IdPhilia,
                        principalTable: "Philia",
                        principalColumn: "IdPhilia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    IdArea = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSubdivision = table.Column<int>(type: "int", nullable: false),
                    NameArea = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IdAreaSAP = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IdAnother = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ObjArea = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.IdArea);
                    table.ForeignKey(
                        name: "FK_Areas_Subdivisions_IdSubdivision",
                        column: x => x.IdSubdivision,
                        principalTable: "Subdivisions",
                        principalColumn: "IdSubdivision",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GasMeasuringObjects",
                columns: table => new
                {
                    IdGMO = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdArea = table.Column<int>(type: "int", nullable: false),
                    NameGMO = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IdGmoSAP = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    IdAnother = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ObjGMO = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GasMeasuringObjects", x => x.IdGMO);
                    table.ForeignKey(
                        name: "FK_GasMeasuringObjects_Areas_IdArea",
                        column: x => x.IdArea,
                        principalTable: "Areas",
                        principalColumn: "IdArea",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeasuringPipes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GmoId = table.Column<long>(type: "bigint", nullable: false),
                    NameMPipe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdAnother = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ObjMPipe = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasuringPipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasuringPipes_GasMeasuringObjects_GmoId",
                        column: x => x.GmoId,
                        principalTable: "GasMeasuringObjects",
                        principalColumn: "IdGMO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PointIOs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPointIOSAP = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IdGMO = table.Column<long>(type: "bigint", nullable: false),
                    NamePointIO = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EIC = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    DateBegin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdAnother = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IdTypePointIO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointIOs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointIOs_GasMeasuringObjects_IdGMO",
                        column: x => x.IdGMO,
                        principalTable: "GasMeasuringObjects",
                        principalColumn: "IdGMO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PointIOs_PointIOTypes_IdTypePointIO",
                        column: x => x.IdTypePointIO,
                        principalTable: "PointIOTypes",
                        principalColumn: "IdTypePointIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PointIOTypes",
                columns: new[] { "IdTypePointIO", "NameTypePointIO" },
                values: new object[,]
                {
                    { 1, "Точка входу" },
                    { 2, "Точка виходу" },
                    { 3, "Точка входу-виходу" },
                    { 4, "Віртуальна точка входу" },
                    { 5, "Віртуальна точка виходу" },
                    { 6, "Віртуальна точка входу-виходу" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Areas_IdSubdivision",
                table: "Areas",
                column: "IdSubdivision");

            migrationBuilder.CreateIndex(
                name: "IX_GasMeasuringObjects_IdArea",
                table: "GasMeasuringObjects",
                column: "IdArea");

            migrationBuilder.CreateIndex(
                name: "IX_MeasuringPipes_GmoId",
                table: "MeasuringPipes",
                column: "GmoId");

            migrationBuilder.CreateIndex(
                name: "IX_PointIOs_IdGMO",
                table: "PointIOs",
                column: "IdGMO");

            migrationBuilder.CreateIndex(
                name: "IX_PointIOs_IdTypePointIO",
                table: "PointIOs",
                column: "IdTypePointIO");

            migrationBuilder.CreateIndex(
                name: "IX_Subdivisions_IdPhilia",
                table: "Subdivisions",
                column: "IdPhilia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasuringPipes");

            migrationBuilder.DropTable(
                name: "PointIOs");

            migrationBuilder.DropTable(
                name: "GasMeasuringObjects");

            migrationBuilder.DropTable(
                name: "PointIOTypes");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "Subdivisions");

            migrationBuilder.DropTable(
                name: "Philia");
        }
    }
}
