using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LostFilmMonitoring.DAO.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Serials",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    LastEpisode = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastEpisodeName = table.Column<string>(type: "TEXT", nullable: true),
                    LastEpisodeTorrentLinkSD = table.Column<string>(type: "TEXT", nullable: true),
                    LastEpisodeTorrentLinkMP4 = table.Column<string>(type: "TEXT", nullable: true),
                    LastEpisodeTorrentLink1080 = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Serials", x => x.Name);
                });

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

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Cookie = table.Column<string>(type: "TEXT", nullable: true),
                    Usess = table.Column<string>(type: "TEXT", nullable: true),
                    Uid = table.Column<string>(type: "TEXT", nullable: true),
                    LastActivity = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Serial = table.Column<string>(type: "TEXT", nullable: false),
                    Quality = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => new { x.UserId, x.Serial });
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Serials");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
