using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ThematicMapCreator.Api.Migrations
{
    public partial class AddLayerStyle : Migration
    {
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "style",
                table: "layer");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "style",
                table: "layer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
