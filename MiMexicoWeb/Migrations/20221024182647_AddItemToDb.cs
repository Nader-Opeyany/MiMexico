using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiMexicoWeb.Migrations
{
    public partial class AddItemToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    price = table.Column<double>(type: "float", nullable: false),
                    meatId = table.Column<int>(type: "int", nullable: false),
                    condimentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.id);
                    table.ForeignKey(
                        name: "FK_Items_Condiments_condimentId",
                        column: x => x.condimentId,
                        principalTable: "Condiments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Items_Meats_meatId",
                        column: x => x.meatId,
                        principalTable: "Meats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_condimentId",
                table: "Items",
                column: "condimentId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_meatId",
                table: "Items",
                column: "meatId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
