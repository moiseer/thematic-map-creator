using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ThematicMapCreator.Api.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "layer");

            migrationBuilder.DropTable(
                "map");

            migrationBuilder.DropTable(
                "user");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "user",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "newid()"),
                    email = table.Column<string>(maxLength: 64),
                    name = table.Column<string>(maxLength: 64),
                    password = table.Column<string>(maxLength: 64)
                },
                constraints: table => { table.PrimaryKey("PK_user", x => x.Id); });

            migrationBuilder.CreateTable(
                "map",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "newid()"),
                    name = table.Column<string>(maxLength: 64),
                    settings = table.Column<string>(),
                    UserId = table.Column<Guid>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_map", x => x.Id);
                    table.ForeignKey(
                        "FK_map_user_UserId",
                        x => x.UserId,
                        "user",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "layer",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "newid()"),
                    data = table.Column<string>(),
                    MapId = table.Column<Guid>(),
                    name = table.Column<string>(maxLength: 64),
                    settings = table.Column<string>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_layer", x => x.Id);
                    table.ForeignKey(
                        "FK_layer_map_MapId",
                        x => x.MapId,
                        "map",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_layer_MapId",
                "layer",
                "MapId");

            migrationBuilder.CreateIndex(
                "IX_map_UserId",
                "map",
                "UserId");
        }
    }
}
