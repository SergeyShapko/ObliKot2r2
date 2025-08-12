using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ObliKot2r2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasuringPipes_GasMeasuringObjects_GmoId",
                table: "MeasuringPipes");

            migrationBuilder.RenameColumn(
                name: "GmoId",
                table: "MeasuringPipes",
                newName: "IdGMO");

            migrationBuilder.RenameIndex(
                name: "IX_MeasuringPipes_GmoId",
                table: "MeasuringPipes",
                newName: "IX_MeasuringPipes_IdGMO");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasuringPipes_GasMeasuringObjects_IdGMO",
                table: "MeasuringPipes",
                column: "IdGMO",
                principalTable: "GasMeasuringObjects",
                principalColumn: "IdGMO",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasuringPipes_GasMeasuringObjects_IdGMO",
                table: "MeasuringPipes");

            migrationBuilder.RenameColumn(
                name: "IdGMO",
                table: "MeasuringPipes",
                newName: "GmoId");

            migrationBuilder.RenameIndex(
                name: "IX_MeasuringPipes_IdGMO",
                table: "MeasuringPipes",
                newName: "IX_MeasuringPipes_GmoId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasuringPipes_GasMeasuringObjects_GmoId",
                table: "MeasuringPipes",
                column: "GmoId",
                principalTable: "GasMeasuringObjects",
                principalColumn: "IdGMO",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
