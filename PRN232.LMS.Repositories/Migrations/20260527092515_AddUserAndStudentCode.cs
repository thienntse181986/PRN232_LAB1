using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRN232.LMS.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAndStudentCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StudentCode",
                table: "Students",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 1,
                column: "StudentCode",
                value: "SE181901");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 2,
                column: "StudentCode",
                value: "SE181902");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 3,
                column: "StudentCode",
                value: "SE181903");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 4,
                column: "StudentCode",
                value: "SE181904");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 5,
                column: "StudentCode",
                value: "SE181905");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 6,
                column: "StudentCode",
                value: "SE181906");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 7,
                column: "StudentCode",
                value: "SE181907");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 8,
                column: "StudentCode",
                value: "SE181908");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 9,
                column: "StudentCode",
                value: "SE181909");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 10,
                column: "StudentCode",
                value: "SE181910");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 11,
                column: "StudentCode",
                value: "SE181911");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 12,
                column: "StudentCode",
                value: "SE181912");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 13,
                column: "StudentCode",
                value: "SE181913");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 14,
                column: "StudentCode",
                value: "SE181914");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 15,
                column: "StudentCode",
                value: "SE181915");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 16,
                column: "StudentCode",
                value: "SE181916");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 17,
                column: "StudentCode",
                value: "SE181917");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 18,
                column: "StudentCode",
                value: "SE181918");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 19,
                column: "StudentCode",
                value: "SE181919");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 20,
                column: "StudentCode",
                value: "SE181920");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 21,
                column: "StudentCode",
                value: "SE181921");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 22,
                column: "StudentCode",
                value: "SE181922");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 23,
                column: "StudentCode",
                value: "SE181923");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 24,
                column: "StudentCode",
                value: "SE181924");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 25,
                column: "StudentCode",
                value: "SE181925");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 26,
                column: "StudentCode",
                value: "SE181926");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 27,
                column: "StudentCode",
                value: "SE181927");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 28,
                column: "StudentCode",
                value: "SE181928");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 29,
                column: "StudentCode",
                value: "SE181929");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 30,
                column: "StudentCode",
                value: "SE181930");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 31,
                column: "StudentCode",
                value: "SE181931");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 32,
                column: "StudentCode",
                value: "SE181932");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 33,
                column: "StudentCode",
                value: "SE181933");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 34,
                column: "StudentCode",
                value: "SE181934");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 35,
                column: "StudentCode",
                value: "SE181935");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 36,
                column: "StudentCode",
                value: "SE181936");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 37,
                column: "StudentCode",
                value: "SE181937");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 38,
                column: "StudentCode",
                value: "SE181938");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 39,
                column: "StudentCode",
                value: "SE181939");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 40,
                column: "StudentCode",
                value: "SE181940");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 41,
                column: "StudentCode",
                value: "SE181941");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 42,
                column: "StudentCode",
                value: "SE181942");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 43,
                column: "StudentCode",
                value: "SE181943");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 44,
                column: "StudentCode",
                value: "SE181944");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 45,
                column: "StudentCode",
                value: "SE181945");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 46,
                column: "StudentCode",
                value: "SE181946");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 47,
                column: "StudentCode",
                value: "SE181947");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 48,
                column: "StudentCode",
                value: "SE181948");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 49,
                column: "StudentCode",
                value: "SE181949");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 50,
                column: "StudentCode",
                value: "SE181950");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "PasswordHash", "RefreshToken", "RefreshTokenExpiryTime", "Role", "Username" },
                values: new object[] { 1, "$2a$11$.BhGtwDVrqDFf4cYtsr7UesAKcUOqZNRG.rbascT9.MIaTMXr0vcC", null, null, "Admin", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentCode",
                table: "Students",
                column: "StudentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Students_StudentCode",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "StudentCode",
                table: "Students");
        }
    }
}
