using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team22.Web.Migrations
{
    public partial class AddDriverIDtoApps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DriverId",
                table: "Application",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Application",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Application_UserId1",
                table: "Application",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Application_AspNetUsers_UserId1",
                table: "Application",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Application_AspNetUsers_UserId1",
                table: "Application");

            migrationBuilder.DropIndex(
                name: "IX_Application_UserId1",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Application");
        }
    }
}
