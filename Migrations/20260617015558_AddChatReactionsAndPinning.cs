using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DOAN_LAPTRINHWEB.Migrations
{
    /// <inheritdoc />
    public partial class AddChatReactionsAndPinning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PinnedAt",
                table: "ChatMessages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PinnedById",
                table: "ChatMessages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChatMessageReactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Emoji = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessageReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessageReactions_ChatMessages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "ChatMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessageReactions_Users_UserId",
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
                value: new DateTime(2026, 6, 17, 1, 55, 57, 826, DateTimeKind.Utc).AddTicks(4356));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 55, 57, 826, DateTimeKind.Utc).AddTicks(4361));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 55, 57, 826, DateTimeKind.Utc).AddTicks(4362));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 55, 57, 826, DateTimeKind.Utc).AddTicks(4363));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 55, 57, 826, DateTimeKind.Utc).AddTicks(4365));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 55, 57, 826, DateTimeKind.Utc).AddTicks(4366));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 55, 57, 826, DateTimeKind.Utc).AddTicks(4367));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 55, 57, 826, DateTimeKind.Utc).AddTicks(4368));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 55, 57, 826, DateTimeKind.Utc).AddTicks(4369));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 55, 57, 826, DateTimeKind.Utc).AddTicks(4370));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 55, 57, 826, DateTimeKind.Utc).AddTicks(4178));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 55, 57, 826, DateTimeKind.Utc).AddTicks(4184));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 55, 57, 826, DateTimeKind.Utc).AddTicks(4186));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 55, 57, 826, DateTimeKind.Utc).AddTicks(4188));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 55, 57, 826, DateTimeKind.Utc).AddTicks(4189));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 55, 57, 826, DateTimeKind.Utc).AddTicks(4190));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 1, 55, 57, 826, DateTimeKind.Utc).AddTicks(4192));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 17, 1, 55, 57, 826, DateTimeKind.Utc).AddTicks(4400), "$2a$11$eDkSvrWtfR9t2mHlviYereSb7NhN6xa0rukx0sHJ261BtiTgVfz6K", new DateTime(2026, 6, 17, 1, 55, 57, 826, DateTimeKind.Utc).AddTicks(4400) });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_PinnedById",
                table: "ChatMessages",
                column: "PinnedById");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessageReactions_MessageId",
                table: "ChatMessageReactions",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessageReactions_UserId",
                table: "ChatMessageReactions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Users_PinnedById",
                table: "ChatMessages",
                column: "PinnedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Users_PinnedById",
                table: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ChatMessageReactions");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_PinnedById",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "PinnedAt",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "PinnedById",
                table: "ChatMessages");

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
        }
    }
}
