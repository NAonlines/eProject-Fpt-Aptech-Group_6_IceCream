using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IceCreamProject.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseNewV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "CartItems",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "CartItems");
        }

    }
}
