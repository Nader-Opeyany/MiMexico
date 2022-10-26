using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiMexicoWeb.Migrations
{
    public partial class AddOrderClassAndMeatsToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Meats",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meats", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SimpleOrderTable",
                columns: table => new
                {
                    orderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customerFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    customerLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    order = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    specialInstructions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    price = table.Column<float>(type: "real", nullable: false),
                    completed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimpleOrderTable", x => x.orderId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Meats");

            migrationBuilder.DropTable(
                name: "SimpleOrderTable");
        }
    }
}
