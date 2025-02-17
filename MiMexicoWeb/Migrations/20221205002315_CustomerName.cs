﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiMexicoWeb.Migrations
{
    public partial class CustomerName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "OrderDetails");
        }
    }
}
