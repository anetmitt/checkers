using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Db.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CheckersOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Width = table.Column<int>(type: "INTEGER", nullable: false),
                    Height = table.Column<int>(type: "INTEGER", nullable: false),
                    WhiteStarts = table.Column<bool>(type: "INTEGER", nullable: false),
                    LoadedOptions = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckersOptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CheckersGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameName = table.Column<string>(type: "TEXT", nullable: true),
                    GameStarted = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GameOver = table.Column<DateTime>(type: "TEXT", nullable: true),
                    GameWinner = table.Column<string>(type: "TEXT", nullable: true),
                    PlayerOneName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    PlayerOneType = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerTwoName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    PlayerTwoType = table.Column<int>(type: "INTEGER", nullable: false),
                    CheckersOptionsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckersGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckersGames_CheckersOptions_CheckersOptionsId",
                        column: x => x.CheckersOptionsId,
                        principalTable: "CheckersOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckersGameStates",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SerializedGameState = table.Column<string>(type: "TEXT", nullable: false),
                    CheckersGameId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckersGameStates", x => x.id);
                    table.ForeignKey(
                        name: "FK_CheckersGameStates_CheckersGames_CheckersGameId",
                        column: x => x.CheckersGameId,
                        principalTable: "CheckersGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheckersGames_CheckersOptionsId",
                table: "CheckersGames",
                column: "CheckersOptionsId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckersGameStates_CheckersGameId",
                table: "CheckersGameStates",
                column: "CheckersGameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckersGameStates");

            migrationBuilder.DropTable(
                name: "CheckersGames");

            migrationBuilder.DropTable(
                name: "CheckersOptions");
        }
    }
}
