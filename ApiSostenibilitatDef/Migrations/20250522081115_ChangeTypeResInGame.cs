using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ApiSostenibilitat.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTypeResInGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MinRes",
                table: "Game",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "MaxRes",
                table: "Game",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.InsertData(
                table: "Game",
                columns: new[] { "Id", "MaxRes", "MinRes", "Name" },
                values: new object[,]
                {
                    { 1, 275, 375, "Reflexes" },
                    { 2, 20000, 20, "Oida" },
                    { 3, 100000000, 50000000, "Vista" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Game",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Game",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Game",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AlterColumn<double>(
                name: "MinRes",
                table: "Game",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "MaxRes",
                table: "Game",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
