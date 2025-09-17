#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.MSSQL.Migrations.Application;

/// <inheritdoc />
public partial class V1_03 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            "Language",
            schema: "Identity",
            table: "Users",
            type: "int",
            maxLength: 2,
            nullable: false,
            defaultValue: 1,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            "Gender",
            schema: "Identity",
            table: "Users",
            type: "int",
            maxLength: 1,
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            "Language",
            schema: "Identity",
            table: "Users",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int",
            oldMaxLength: 2,
            oldDefaultValue: 1);

        migrationBuilder.AlterColumn<int>(
            "Gender",
            schema: "Identity",
            table: "Users",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int",
            oldMaxLength: 1,
            oldDefaultValue: 0);
    }
}
