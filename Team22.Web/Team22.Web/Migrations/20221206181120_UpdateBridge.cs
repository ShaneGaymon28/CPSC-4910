using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team22.Web.Migrations
{
    public partial class UpdateBridge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bridges_Users_UserId",
                table: "Bridges");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Bridges",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Bridges",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Bridges_AppUserId",
                table: "Bridges",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bridges_AspNetUsers_AppUserId",
                table: "Bridges",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bridges_Users_UserId",
                table: "Bridges",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bridges_AspNetUsers_AppUserId",
                table: "Bridges");

            migrationBuilder.DropForeignKey(
                name: "FK_Bridges_Users_UserId",
                table: "Bridges");

            migrationBuilder.DropIndex(
                name: "IX_Bridges_AppUserId",
                table: "Bridges");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Bridges");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Bridges",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bridges_Users_UserId",
                table: "Bridges",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
