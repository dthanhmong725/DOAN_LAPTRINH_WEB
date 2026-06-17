using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DOAN_LAPTRINHWEB.Migrations
{
    /// <inheritdoc />
    public partial class AddReputationHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReputationHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    PointsChange = table.Column<int>(type: "int", nullable: false),
                    TotalPointsAfter = table.Column<int>(type: "int", nullable: false),
                    RankAfter = table.Column<int>(type: "int", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: true),
                    CommentId = table.Column<int>(type: "int", nullable: true),
                    ActorId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReputationHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReputationHistories_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReputationHistories_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReputationHistories_Users_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReputationHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 48, 47, 706, DateTimeKind.Utc).AddTicks(6391));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 48, 47, 706, DateTimeKind.Utc).AddTicks(6395));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 48, 47, 706, DateTimeKind.Utc).AddTicks(6397));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 48, 47, 706, DateTimeKind.Utc).AddTicks(6398));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 48, 47, 706, DateTimeKind.Utc).AddTicks(6399));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 48, 47, 706, DateTimeKind.Utc).AddTicks(6400));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 48, 47, 706, DateTimeKind.Utc).AddTicks(6402));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 48, 47, 706, DateTimeKind.Utc).AddTicks(6403));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 48, 47, 706, DateTimeKind.Utc).AddTicks(6404));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 48, 47, 706, DateTimeKind.Utc).AddTicks(6405));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 48, 47, 706, DateTimeKind.Utc).AddTicks(6224));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 48, 47, 706, DateTimeKind.Utc).AddTicks(6230));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 48, 47, 706, DateTimeKind.Utc).AddTicks(6232));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 48, 47, 706, DateTimeKind.Utc).AddTicks(6233));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 48, 47, 706, DateTimeKind.Utc).AddTicks(6235));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 48, 47, 706, DateTimeKind.Utc).AddTicks(6236));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 48, 47, 706, DateTimeKind.Utc).AddTicks(6237));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 17, 1, 48, 47, 706, DateTimeKind.Utc).AddTicks(6432), "$2a$11$diO5tFMcZxyeaw5ygEV9IeNbhgEXCpAyAKClnctBRmSOSMfFLHSIW", new DateTime(2026, 6, 17, 1, 48, 47, 706, DateTimeKind.Utc).AddTicks(6432) });

            migrationBuilder.CreateIndex(
                name: "IX_ReputationHistories_ActorId",
                table: "ReputationHistories",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReputationHistories_CommentId",
                table: "ReputationHistories",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReputationHistories_PostId",
                table: "ReputationHistories",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_ReputationHistories_UserId_CreatedAt",
                table: "ReputationHistories",
                columns: new[] { "UserId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReputationHistories");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 12, 7, 33, 11, 694, DateTimeKind.Utc).AddTicks(3574));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 12, 7, 33, 11, 694, DateTimeKind.Utc).AddTicks(3580));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 12, 7, 33, 11, 694, DateTimeKind.Utc).AddTicks(3582));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 12, 7, 33, 11, 694, DateTimeKind.Utc).AddTicks(3584));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 12, 7, 33, 11, 694, DateTimeKind.Utc).AddTicks(3585));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 12, 7, 33, 11, 694, DateTimeKind.Utc).AddTicks(3587));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 12, 7, 33, 11, 694, DateTimeKind.Utc).AddTicks(3588));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 12, 7, 33, 11, 694, DateTimeKind.Utc).AddTicks(3590));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 12, 7, 33, 11, 694, DateTimeKind.Utc).AddTicks(3591));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 12, 7, 33, 11, 694, DateTimeKind.Utc).AddTicks(3593));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 12, 7, 33, 11, 694, DateTimeKind.Utc).AddTicks(3275));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 12, 7, 33, 11, 694, DateTimeKind.Utc).AddTicks(3283));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 12, 7, 33, 11, 694, DateTimeKind.Utc).AddTicks(3286));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 12, 7, 33, 11, 694, DateTimeKind.Utc).AddTicks(3289));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 12, 7, 33, 11, 694, DateTimeKind.Utc).AddTicks(3290));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 12, 7, 33, 11, 694, DateTimeKind.Utc).AddTicks(3292));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 12, 7, 33, 11, 694, DateTimeKind.Utc).AddTicks(3294));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 12, 7, 33, 11, 694, DateTimeKind.Utc).AddTicks(3628), "$2a$11$5EdPaFMvj9zhrfSZiA94pO1aYfo8bYpwFF3geBJgfEoiypbJmBKbW", new DateTime(2026, 6, 12, 7, 33, 11, 694, DateTimeKind.Utc).AddTicks(3629) });
        }
    }
}
