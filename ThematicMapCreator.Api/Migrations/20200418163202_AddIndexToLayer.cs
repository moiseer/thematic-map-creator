using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ThematicMapCreator.Api.Migrations
{
    public partial class AddIndexToLayer : Migration
    {
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_email",
                table: "user");

            migrationBuilder.DropIndex(
                name: "IX_user_name",
                table: "user");

            migrationBuilder.DropColumn(
                name: "index",
                table: "layer");

            migrationBuilder.DropColumn(
                name: "visible",
                table: "layer");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "index",
                table: "layer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "visible",
                table: "layer",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_user_email",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_name",
                table: "user",
                column: "name",
                unique: true);
        }
    }
}
