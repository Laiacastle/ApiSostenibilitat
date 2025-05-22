using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiSostenibilitat.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNameGameToType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Game",
                newName: "Type");

            migrationBuilder.AlterColumn<int>(
                name: "FiResult",
                table: "Result",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Game",
                newName: "Name");

            migrationBuilder.AlterColumn<double>(
                name: "FiResult",
                table: "Result",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
