using Microsoft.EntityFrameworkCore.Migrations;

namespace ThematicMapCreator.Api.Migrations
{
    public partial class UserPasswordHash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                table: "user",
                name: "password",
                newName: "password_hash");

            migrationBuilder.Sql(@"
update [user]
set [password_hash] = CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', CONVERT(VARCHAR(64), [password_hash])), 2)
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                table: "user",
                name: "password_hash",
                newName: "password");
        }
    }
}
