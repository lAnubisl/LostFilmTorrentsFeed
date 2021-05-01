using Microsoft.EntityFrameworkCore.Migrations;

namespace LostFilmMonitoring.DAO.Migrations
{
    public partial class Refactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.RenameColumn(
                name: "Serial",
                table: "Subscriptions",
                newName: "SeriesName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Feeds",
                newName: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SeriesName",
                table: "Subscriptions",
                newName: "Serial");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Feeds",
                newName: "Id");

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Name);
                });
        }
    }
}
