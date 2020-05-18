using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ThematicMapCreator.Api.Migrations
{
    public partial class AddLayerOptionsTable : Migration
    {
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_layer_map_map_id",
                table: "layer");

            migrationBuilder.DropForeignKey(
                name: "FK_map_user_user_id",
                table: "map");

            migrationBuilder.DropTable(
                name: "layer_options");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "user",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "map",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "map",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_map_user_id",
                table: "map",
                newName: "IX_map_UserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "layer",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "map_id",
                table: "layer",
                newName: "MapId");

            migrationBuilder.RenameIndex(
                name: "IX_layer_map_id",
                table: "layer",
                newName: "IX_layer_MapId");

            migrationBuilder.AddColumn<string>(
                name: "settings",
                table: "layer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_layer_map_MapId",
                table: "layer",
                column: "MapId",
                principalTable: "map",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_map_user_UserId",
                table: "map",
                column: "UserId",
                principalTable: "user",
                principalColumn: "Id");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_layer_map_MapId",
                table: "layer");

            migrationBuilder.DropForeignKey(
                name: "FK_map_user_UserId",
                table: "map");

            migrationBuilder.DropColumn(
                name: "settings",
                table: "layer");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "user",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "map",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "map",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_map_UserId",
                table: "map",
                newName: "IX_map_user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "layer",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "MapId",
                table: "layer",
                newName: "map_id");

            migrationBuilder.RenameIndex(
                name: "IX_layer_MapId",
                table: "layer",
                newName: "IX_layer_map_id");

            migrationBuilder.CreateTable(
                name: "layer_options",
                columns: table => new
                {
                    layer_id = table.Column<Guid>(nullable: false),
                    type = table.Column<int>(nullable: false, defaultValue: 0)
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

            migrationBuilder.AddForeignKey(
                name: "FK_layer_map_map_id",
                table: "layer",
                column: "map_id",
                principalTable: "map",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_map_user_user_id",
                table: "map",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id");
        }
    }
}
