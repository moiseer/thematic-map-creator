using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ThematicMapCreator.Api.Migrations
{
    public partial class AddMapDescription : Migration
    {
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "map");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "map",
                maxLength: 1024,
                nullable: true);
        }
    }
}
