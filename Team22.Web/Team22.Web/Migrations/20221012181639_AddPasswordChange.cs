using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team22.Web.Migrations
{
    public partial class AddPasswordChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Guid",
                table: "Verification",
                newName: "Secret");

            migrationBuilder.AddColumn<int>(
                name: "PasswordChangeId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PasswordChanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Forced = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Secret = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Expiration = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordChanges", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PasswordChangeId",
                table: "Users",
                column: "PasswordChangeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_PasswordChanges_PasswordChangeId",
                table: "Users",
                column: "PasswordChangeId",
                principalTable: "PasswordChanges",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_PasswordChanges_PasswordChangeId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "PasswordChanges");

            migrationBuilder.DropIndex(
                name: "IX_Users_PasswordChangeId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordChangeId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Secret",
                table: "Verification",
                newName: "Guid");
        }
    }
}
