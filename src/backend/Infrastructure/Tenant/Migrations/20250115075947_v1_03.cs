using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvrenDev.Infrastructure.Tenant.Migrations
{
    /// <inheritdoc />
    public partial class v1_03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminEmail",
                schema: "Tenant",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedTime",
                schema: "Tenant",
                table: "Tenants",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Creator",
                schema: "Tenant",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                schema: "Tenant",
                table: "Tenants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Deleter",
                schema: "Tenant",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletionTime",
                schema: "Tenant",
                table: "Tenants",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "Tenant",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModifiedTime",
                schema: "Tenant",
                table: "Tenants",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Modifier",
                schema: "Tenant",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidUntil",
                schema: "Tenant",
                table: "Tenants",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminEmail",
                schema: "Tenant",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                schema: "Tenant",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Creator",
                schema: "Tenant",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Deleted",
                schema: "Tenant",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Deleter",
                schema: "Tenant",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                schema: "Tenant",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "Tenant",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "ModifiedTime",
                schema: "Tenant",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Modifier",
                schema: "Tenant",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "ValidUntil",
                schema: "Tenant",
                table: "Tenants");
        }
    }
}
