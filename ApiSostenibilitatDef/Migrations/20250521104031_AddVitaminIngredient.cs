using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiSostenibilitat.Migrations
{
    /// <inheritdoc />
    public partial class AddVitaminIngredient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientVitamin_Ingredient_IngredientId",
                table: "IngredientVitamin");

            migrationBuilder.RenameColumn(
                name: "IngredientId",
                table: "IngredientVitamin",
                newName: "IngredientsId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientVitamin_Ingredient_IngredientsId",
                table: "IngredientVitamin",
                column: "IngredientsId",
                principalTable: "Ingredient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientVitamin_Ingredient_IngredientsId",
                table: "IngredientVitamin");

            migrationBuilder.RenameColumn(
                name: "IngredientsId",
                table: "IngredientVitamin",
                newName: "IngredientId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientVitamin_Ingredient_IngredientId",
                table: "IngredientVitamin",
                column: "IngredientId",
                principalTable: "Ingredient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
