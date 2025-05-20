using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiSostenibilitat.Migrations
{
    /// <inheritdoc />
    public partial class ChangeResultRecipesDiet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diet_AspNetUsers_UserId",
                table: "Diet");

            migrationBuilder.DropIndex(
                name: "IX_Diet_UserId",
                table: "Diet");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Diet",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.CreateIndex(
                name: "IX_Diet_UserId",
                table: "Diet",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Diet_AspNetUsers_UserId",
                table: "Diet",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diet_AspNetUsers_UserId",
                table: "Diet");

            migrationBuilder.DropIndex(
                name: "IX_Diet_UserId",
                table: "Diet");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Diet",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Diet_UserId",
                table: "Diet",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Diet_AspNetUsers_UserId",
                table: "Diet",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
