using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team22.Web.Migrations
{
    public partial class UpdateSponsor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AcceptingApps",
                table: "Sponsors",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptingApps",
                table: "Sponsors");
        }
    }
}
