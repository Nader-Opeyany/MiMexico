using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiMexicoWeb.Migrations
{
    public partial class addItemBeverage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Beverage",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Beverage",
                table: "Items");
        }
    }
}
