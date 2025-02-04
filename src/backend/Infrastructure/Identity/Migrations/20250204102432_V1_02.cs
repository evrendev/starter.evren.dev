using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvrenDev.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class V1_02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedTime",
                schema: "Identity",
                table: "Users",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Creator",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Deleter",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletionTime",
                schema: "Identity",
                table: "Users",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModifiedTime",
                schema: "Identity",
                table: "Users",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Modifier",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedTime",
                schema: "Identity",
                table: "Roles",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Creator",
                schema: "Identity",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Deleter",
                schema: "Identity",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletionTime",
                schema: "Identity",
                table: "Roles",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModifiedTime",
                schema: "Identity",
                table: "Roles",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Modifier",
                schema: "Identity",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTime",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Creator",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Deleter",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ModifiedTime",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Modifier",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                schema: "Identity",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Creator",
                schema: "Identity",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Deleter",
                schema: "Identity",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                schema: "Identity",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "ModifiedTime",
                schema: "Identity",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Modifier",
                schema: "Identity",
                table: "Roles");
        }
    }
}
