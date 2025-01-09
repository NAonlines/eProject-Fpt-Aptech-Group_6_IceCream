using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IceCreamProject.Migrations
{
    /// <inheritdoc />
    public partial class newmembership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MembershipPayments");

            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "Books");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "PaymentMember",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMember_OrderId",
                table: "PaymentMember",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMember_Orders_OrderId",
                table: "PaymentMember",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMember_Orders_OrderId",
                table: "PaymentMember");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMember_OrderId",
                table: "PaymentMember");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "PaymentMember");

            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MembershipPayments",
                columns: table => new
                {
                    MembershipPaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MembershipEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MembershipStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MembershipType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrdersId = table.Column<int>(type: "int", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipPayments", x => x.MembershipPaymentId);
                    table.ForeignKey(
                        name: "FK_MembershipPayments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MembershipPayments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MembershipPayments_OrderId",
                table: "MembershipPayments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipPayments_UserId",
                table: "MembershipPayments",
                column: "UserId");
        }
    }
}
