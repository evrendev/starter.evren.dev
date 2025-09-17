#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.MSSQL.Migrations.Application;

/// <inheritdoc />
public partial class V1_02 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            "Gender",
            schema: "Identity",
            table: "Users",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            "Language",
            schema: "Identity",
            table: "Users",
            type: "int",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            "Gender",
            schema: "Identity",
            table: "Users");

        migrationBuilder.DropColumn(
            "Language",
            schema: "Identity",
            table: "Users");
    }
}
