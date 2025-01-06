using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IceCreamProject.Migrations
{
    /// <inheritdoc />
    public partial class Updatebasev2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Recipes_BookId",
                table: "Recipes");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Books",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_BookId",
                table: "Recipes",
                column: "BookId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Recipes_BookId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Books");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_BookId",
                table: "Recipes",
                column: "BookId",
                unique: true,
                filter: "[BookId] IS NOT NULL");
        }
    }
}
