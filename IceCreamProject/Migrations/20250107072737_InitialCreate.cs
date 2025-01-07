using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IceCreamProject.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MemberPrice",
                columns: table => new
                {
                    IDMemberShipPrice = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NamePrice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberPrice", x => x.IDMemberShipPrice);
                });

            migrationBuilder.CreateTable(
                name: "Memberships",
                columns: table => new
                {
                    IDMembership = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PriceMemberID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberships", x => x.IDMembership);
                    table.ForeignKey(
                        name: "FK_Memberships_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Memberships_MemberPrice_PriceMemberID",
                        column: x => x.PriceMemberID,
                        principalTable: "MemberPrice",
                        principalColumn: "IDMemberShipPrice",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMember",
                columns: table => new
                {
                    IDPayment = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PriceMemberID = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMember", x => x.IDPayment);
                    table.ForeignKey(
                        name: "FK_PaymentMember_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentMember_MemberPrice_PriceMemberID",
                        column: x => x.PriceMemberID,
                        principalTable: "MemberPrice",
                        principalColumn: "IDMemberShipPrice",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_PriceMemberID",
                table: "Memberships",
                column: "PriceMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_UserID",
                table: "Memberships",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMember_PriceMemberID",
                table: "PaymentMember",
                column: "PriceMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMember_UserID",
                table: "PaymentMember",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Memberships");

            migrationBuilder.DropTable(
                name: "PaymentMember");

            migrationBuilder.DropTable(
                name: "MemberPrice");

            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "Books");
        }
    }
}
