using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmacy_System.Migrations
{
    /// <inheritdoc />
    public partial class AddLoginLockout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailedLoginAttempts",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockedUntil",
                table: "users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "UserID",
                keyValue: 1,
                columns: new[] { "FailedLoginAttempts", "LockedUntil" },
                values: new object[] { 0, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedLoginAttempts",
                table: "users");

            migrationBuilder.DropColumn(
                name: "LockedUntil",
                table: "users");
        }
    }
}
