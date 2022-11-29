using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiMexicoWeb.Migrations
{
    public partial class unwatnedColumnCleanUp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("OrderDate", "OrderHeaders");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
