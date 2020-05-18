using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ThematicMapCreator.Api.Migrations
{
    public partial class AddUniqueValuesStyleOptions : Migration
    {
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "style",
                table: "layer",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "style",
                table: "layer");
        }
    }
}
