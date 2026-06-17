using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DOAN_LAPTRINHWEB.Migrations
{
    /// <inheritdoc />
    public partial class SeedMoreUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 3, 43, 35, 610, DateTimeKind.Utc).AddTicks(1538));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 3, 43, 35, 610, DateTimeKind.Utc).AddTicks(1541));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 3, 43, 35, 610, DateTimeKind.Utc).AddTicks(1543));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 3, 43, 35, 610, DateTimeKind.Utc).AddTicks(1544));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 3, 43, 35, 610, DateTimeKind.Utc).AddTicks(1545));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 3, 43, 35, 610, DateTimeKind.Utc).AddTicks(1546));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 3, 43, 35, 610, DateTimeKind.Utc).AddTicks(1547));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 3, 43, 35, 610, DateTimeKind.Utc).AddTicks(1548));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 3, 43, 35, 610, DateTimeKind.Utc).AddTicks(1550));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 3, 43, 35, 610, DateTimeKind.Utc).AddTicks(1551));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 3, 43, 35, 610, DateTimeKind.Utc).AddTicks(1365));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 3, 43, 35, 610, DateTimeKind.Utc).AddTicks(1370));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 3, 43, 35, 610, DateTimeKind.Utc).AddTicks(1373));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 3, 43, 35, 610, DateTimeKind.Utc).AddTicks(1374));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 3, 43, 35, 610, DateTimeKind.Utc).AddTicks(1375));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 3, 43, 35, 610, DateTimeKind.Utc).AddTicks(1377));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 3, 43, 35, 610, DateTimeKind.Utc).AddTicks(1378));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 17, 3, 43, 35, 846, DateTimeKind.Utc).AddTicks(9778), "$2a$11$pdjde9PtgzqcGb0lSDAYiOuM7Pshj5NWyjNcLNsRVWTSXfGqZWc36", new DateTime(2026, 6, 17, 3, 43, 35, 846, DateTimeKind.Utc).AddTicks(9782) });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "BanReason", "Bio", "CoverPhotoUrl", "CreatedAt", "DisplayName", "Email", "EmailVerificationToken", "EmailVerificationTokenExpiry", "FailedLoginAttempts", "IsActive", "IsBanned", "IsEmailVerified", "LastLoginAt", "LastLoginIp", "LockoutEnd", "PasswordHash", "Rank", "ReputationPoints", "ResetToken", "ResetTokenExpiry", "Role", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { 101, null, null, "System Administrator", null, new DateTime(2026, 6, 17, 3, 43, 35, 846, DateTimeKind.Utc).AddTicks(9954), "Admin 2", "admin2@cyberforum.local", null, null, 0, true, false, true, null, null, null, "$2a$11$pdjde9PtgzqcGb0lSDAYiOuM7Pshj5NWyjNcLNsRVWTSXfGqZWc36", 5, 5000, null, null, 2, new DateTime(2026, 6, 17, 3, 43, 35, 846, DateTimeKind.Utc).AddTicks(9954), "admin2" },
                    { 102, null, null, "System Administrator", null, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(696), "Admin 3", "admin3@cyberforum.local", null, null, 0, true, false, true, null, null, null, "$2a$11$pdjde9PtgzqcGb0lSDAYiOuM7Pshj5NWyjNcLNsRVWTSXfGqZWc36", 5, 5000, null, null, 2, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(696), "admin3" },
                    { 103, null, null, "System Administrator", null, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(720), "Admin 4", "admin4@cyberforum.local", null, null, 0, true, false, true, null, null, null, "$2a$11$pdjde9PtgzqcGb0lSDAYiOuM7Pshj5NWyjNcLNsRVWTSXfGqZWc36", 5, 5000, null, null, 2, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(720), "admin4" },
                    { 104, null, null, "System Administrator", null, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(855), "Admin 5", "admin5@cyberforum.local", null, null, 0, true, false, true, null, null, null, "$2a$11$pdjde9PtgzqcGb0lSDAYiOuM7Pshj5NWyjNcLNsRVWTSXfGqZWc36", 5, 5000, null, null, 2, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(855), "admin5" },
                    { 201, null, null, "Cybersecurity Enthusiast", null, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(901), "Member 1", "user1@cyberforum.local", null, null, 0, true, false, true, null, null, null, "$2a$11$rD1zN2/JlxsqOTTn0qp5vu4Bllhcqy/.dBO094j7h5DKuZTjRDChO", 0, 10, null, null, 0, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(901), "user1" },
                    { 202, null, null, "Cybersecurity Enthusiast", null, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(915), "Member 2", "user2@cyberforum.local", null, null, 0, true, false, true, null, null, null, "$2a$11$rD1zN2/JlxsqOTTn0qp5vu4Bllhcqy/.dBO094j7h5DKuZTjRDChO", 0, 20, null, null, 0, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(915), "user2" },
                    { 203, null, null, "Cybersecurity Enthusiast", null, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(921), "Member 3", "user3@cyberforum.local", null, null, 0, true, false, true, null, null, null, "$2a$11$rD1zN2/JlxsqOTTn0qp5vu4Bllhcqy/.dBO094j7h5DKuZTjRDChO", 0, 30, null, null, 0, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(922), "user3" },
                    { 204, null, null, "Cybersecurity Enthusiast", null, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(927), "Member 4", "user4@cyberforum.local", null, null, 0, true, false, true, null, null, null, "$2a$11$rD1zN2/JlxsqOTTn0qp5vu4Bllhcqy/.dBO094j7h5DKuZTjRDChO", 0, 40, null, null, 0, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(927), "user4" },
                    { 205, null, null, "Cybersecurity Enthusiast", null, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(943), "Member 5", "user5@cyberforum.local", null, null, 0, true, false, true, null, null, null, "$2a$11$rD1zN2/JlxsqOTTn0qp5vu4Bllhcqy/.dBO094j7h5DKuZTjRDChO", 0, 50, null, null, 0, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(943), "user5" },
                    { 206, null, null, "Cybersecurity Enthusiast", null, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(950), "Member 6", "user6@cyberforum.local", null, null, 0, true, false, true, null, null, null, "$2a$11$rD1zN2/JlxsqOTTn0qp5vu4Bllhcqy/.dBO094j7h5DKuZTjRDChO", 0, 60, null, null, 0, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(950), "user6" },
                    { 207, null, null, "Cybersecurity Enthusiast", null, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(995), "Member 7", "user7@cyberforum.local", null, null, 0, true, false, true, null, null, null, "$2a$11$rD1zN2/JlxsqOTTn0qp5vu4Bllhcqy/.dBO094j7h5DKuZTjRDChO", 0, 70, null, null, 0, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(995), "user7" },
                    { 208, null, null, "Cybersecurity Enthusiast", null, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(1001), "Member 8", "user8@cyberforum.local", null, null, 0, true, false, true, null, null, null, "$2a$11$rD1zN2/JlxsqOTTn0qp5vu4Bllhcqy/.dBO094j7h5DKuZTjRDChO", 0, 80, null, null, 0, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(1002), "user8" },
                    { 209, null, null, "Cybersecurity Enthusiast", null, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(1040), "Member 9", "user9@cyberforum.local", null, null, 0, true, false, true, null, null, null, "$2a$11$rD1zN2/JlxsqOTTn0qp5vu4Bllhcqy/.dBO094j7h5DKuZTjRDChO", 0, 90, null, null, 0, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(1040), "user9" },
                    { 210, null, null, "Cybersecurity Enthusiast", null, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(1047), "Member 10", "user10@cyberforum.local", null, null, 0, true, false, true, null, null, null, "$2a$11$rD1zN2/JlxsqOTTn0qp5vu4Bllhcqy/.dBO094j7h5DKuZTjRDChO", 0, 100, null, null, 0, new DateTime(2026, 6, 17, 3, 43, 35, 847, DateTimeKind.Utc).AddTicks(1057), "user10" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 201);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 202);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 203);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 204);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 205);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 206);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 207);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 208);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 209);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 210);

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
        }
    }
}
