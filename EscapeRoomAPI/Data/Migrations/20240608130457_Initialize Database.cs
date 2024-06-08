using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EscapeRoomAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitializeDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameSession",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SessionCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    TotalPlayer = table.Column<int>(type: "int", nullable: false),
                    IsWaiting = table.Column<bool>(type: "bit", nullable: false),
                    IsEnd = table.Column<bool>(type: "bit", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    Hint = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GameSess__C9F492901B46D757", x => x.SessionId);
                });

            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false, defaultValueSql: "(CONVERT([nvarchar](36),newid()))"),
                    Username = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Player__3213E83FCAD2F2D2", x => x.id);
                    table.UniqueConstraint("AK_Player_PlayerId", x => x.PlayerId);
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false, defaultValueSql: "(CONVERT([nvarchar](36),newid()))"),
                    QuestionDesc = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyDigit = table.Column<int>(type: "int", nullable: false),
                    IsHard = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Question__3213E83F75227A3C", x => x.id);
                    table.UniqueConstraint("AK_Question_QuestionId", x => x.QuestionId);
                });

            migrationBuilder.CreateTable(
                name: "Leaderboard",
                columns: table => new
                {
                    LeaderBoardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    TotalRightAnswer = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Leaderbo__91D442149723193C", x => x.LeaderBoardId);
                    table.ForeignKey(
                        name: "FK_Leaderboard_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "PlayerId");
                    table.ForeignKey(
                        name: "FK_Leaderboard_SessionId",
                        column: x => x.SessionId,
                        principalTable: "GameSession",
                        principalColumn: "SessionId");
                });

            migrationBuilder.CreateTable(
                name: "PlayerGameSession",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    IsHost = table.Column<bool>(type: "bit", nullable: false),
                    IsReady = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PlayerGa__5D5075DCB278F05E", x => new { x.SessionId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_PlayerGameSession_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "PlayerId");
                    table.ForeignKey(
                        name: "FK_PlayerGameSession_SessionId",
                        column: x => x.SessionId,
                        principalTable: "GameSession",
                        principalColumn: "SessionId");
                });

            migrationBuilder.CreateTable(
                name: "QuestionAnswer",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionAnswerId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false, defaultValueSql: "(CONVERT([nvarchar](36),newid()))"),
                    Answer = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    QuestionId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    IsTrue = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Question__3213E83FFE50928A", x => x.id);
                    table.UniqueConstraint("AK_QuestionAnswer_QuestionAnswerId", x => x.QuestionAnswerId);
                    table.ForeignKey(
                        name: "FK_QuestionAnswer_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "QuestionId");
                });

            migrationBuilder.CreateTable(
                name: "PlayerGameAnswer",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    QuestionId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    SelectAnswerId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PlayerGa__3213E83F4FCA4DD2", x => x.id);
                    table.ForeignKey(
                        name: "FK_PlayerGameAnswer_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "PlayerId");
                    table.ForeignKey(
                        name: "FK_PlayerGameAnswer_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "QuestionId");
                    table.ForeignKey(
                        name: "FK_PlayerGameAnswer_SelectAnswerId",
                        column: x => x.SelectAnswerId,
                        principalTable: "QuestionAnswer",
                        principalColumn: "QuestionAnswerId");
                    table.ForeignKey(
                        name: "FK_PlayerGameAnswer_SessionId",
                        column: x => x.SessionId,
                        principalTable: "GameSession",
                        principalColumn: "SessionId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Leaderboard_PlayerId",
                table: "Leaderboard",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Leaderboard_SessionId",
                table: "Leaderboard",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "UQ__Player__4A4E74C95FB65E42",
                table: "Player",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Player__536C85E4025B578D",
                table: "Player",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGameAnswer_PlayerId",
                table: "PlayerGameAnswer",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGameAnswer_QuestionId",
                table: "PlayerGameAnswer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGameAnswer_SelectAnswerId",
                table: "PlayerGameAnswer",
                column: "SelectAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGameAnswer_SessionId",
                table: "PlayerGameAnswer",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGameSession_PlayerId",
                table: "PlayerGameSession",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "UQ__Question__0DC06FADEAB8E116",
                table: "Question",
                column: "QuestionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswer_QuestionId",
                table: "QuestionAnswer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "UQ__Question__86BEDFCE7B684988",
                table: "QuestionAnswer",
                column: "QuestionAnswerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Leaderboard");

            migrationBuilder.DropTable(
                name: "PlayerGameAnswer");

            migrationBuilder.DropTable(
                name: "PlayerGameSession");

            migrationBuilder.DropTable(
                name: "QuestionAnswer");

            migrationBuilder.DropTable(
                name: "Player");

            migrationBuilder.DropTable(
                name: "GameSession");

            migrationBuilder.DropTable(
                name: "Question");
        }
    }
}
