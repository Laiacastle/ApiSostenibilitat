using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ApiSostenibilitat.Migrations
{
    /// <inheritdoc />
    public partial class AddVitamIngredient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vitamins_Ingredient_IngredientId",
                table: "Vitamins");

            migrationBuilder.DropIndex(
                name: "IX_Vitamins_IngredientId",
                table: "Vitamins");

            migrationBuilder.DropColumn(
                name: "IngredientId",
                table: "Vitamins");

            migrationBuilder.CreateTable(
                name: "IngredientVitamin",
                columns: table => new
                {
                    IngredientId = table.Column<int>(type: "int", nullable: false),
                    VitaminsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientVitamin", x => new { x.IngredientId, x.VitaminsId });
                    table.ForeignKey(
                        name: "FK_IngredientVitamin_Ingredient_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientVitamin_Vitamins_VitaminsId",
                        column: x => x.VitaminsId,
                        principalTable: "Vitamins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Ingredient",
                columns: new[] { "Id", "Calories", "EatForms", "Fiber", "Name" },
                values: new object[,]
                {
                    { 1, 41.0, "[\"Crua\",\"Cuita\",\"Ratllada\",\"Batuda\"]", 2.7999999999999998, "Pastanaga" },
                    { 2, 23.0, "[\"Cuita\"]", 2.2000000000000002, "Espinaca" },
                    { 3, 89.0, "[\"Cru\",\"Fregit\",\"Batut\"]", 2.6000000000000001, "Plátan" },
                    { 4, 160.0, "[\"Cru\",\"Batut\"]", 6.7000000000000002, "Aguacate" },
                    { 5, 34.0, "[\"Cuita\",\"Batut\"]", 2.6000000000000001, "Brócoli" },
                    { 6, 18.0, "[\"Cru\",\"Cuinat\",\"Batut\"]", 1.2, "Tomaquet" },
                    { 7, 579.0, "[\"Crua\",\"Ratllada\"]", 12.5, "Almendra" }
                });

            migrationBuilder.InsertData(
                table: "Vitamins",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "A" },
                    { 2, "C" },
                    { 3, "K" },
                    { 4, "B6" },
                    { 5, "E" },
                    { 6, "D" }
                });

            migrationBuilder.InsertData(
                table: "IngredientVitamin",
                columns: new[] { "IngredientId", "VitaminsId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 2, 3 },
                    { 3, 4 },
                    { 4, 5 },
                    { 5, 2 },
                    { 5, 3 },
                    { 6, 1 },
                    { 6, 2 },
                    { 7, 5 },
                    { 7, 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngredientVitamin_VitaminsId",
                table: "IngredientVitamin",
                column: "VitaminsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredientVitamin");

            migrationBuilder.DeleteData(
                table: "Ingredient",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Ingredient",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Ingredient",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Ingredient",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Ingredient",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Ingredient",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Ingredient",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Vitamins",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Vitamins",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Vitamins",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Vitamins",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Vitamins",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Vitamins",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.AddColumn<int>(
                name: "IngredientId",
                table: "Vitamins",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vitamins_IngredientId",
                table: "Vitamins",
                column: "IngredientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vitamins_Ingredient_IngredientId",
                table: "Vitamins",
                column: "IngredientId",
                principalTable: "Ingredient",
                principalColumn: "Id");
        }
    }
}
