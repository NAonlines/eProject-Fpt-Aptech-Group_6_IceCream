using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IceCreamProject.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseNewV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrdersId",
                table: "CartItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrdersId",
                table: "CartItems",
                type: "int",
                nullable: true);
        }
    }
}
