using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiSostenibilitat.Migrations
{
    /// <inheritdoc />
    public partial class AddVitaminsToDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vitamin_Ingredient_IngredientId",
                table: "Vitamin");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vitamin",
                table: "Vitamin");

            migrationBuilder.RenameTable(
                name: "Vitamin",
                newName: "Vitamins");

            migrationBuilder.RenameIndex(
                name: "IX_Vitamin_IngredientId",
                table: "Vitamins",
                newName: "IX_Vitamins_IngredientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vitamins",
                table: "Vitamins",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vitamins_Ingredient_IngredientId",
                table: "Vitamins",
                column: "IngredientId",
                principalTable: "Ingredient",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vitamins_Ingredient_IngredientId",
                table: "Vitamins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vitamins",
                table: "Vitamins");

            migrationBuilder.RenameTable(
                name: "Vitamins",
                newName: "Vitamin");

            migrationBuilder.RenameIndex(
                name: "IX_Vitamins_IngredientId",
                table: "Vitamin",
                newName: "IX_Vitamin_IngredientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vitamin",
                table: "Vitamin",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vitamin_Ingredient_IngredientId",
                table: "Vitamin",
                column: "IngredientId",
                principalTable: "Ingredient",
                principalColumn: "Id");
        }
    }
}
