using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ThematicMapCreator.Api.Migrations
{
    public partial class DropLayerOptionsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "layer_options");

            migrationBuilder.DropColumn(
                name: "settings",
                table: "map");

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "layer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "type",
                table: "layer");

            migrationBuilder.AddColumn<string>(
                name: "settings",
                table: "map",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "layer_options",
                columns: table => new
                {
                    layer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_layer_options", x => x.layer_id);
                    table.ForeignKey(
                        name: "FK_layer_options_layer_layer_id",
                        column: x => x.layer_id,
                        principalTable: "layer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
