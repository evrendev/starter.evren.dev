#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.PostgreSQL.Migrations.Tenant;

/// <inheritdoc />
public partial class V102 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTime>(
            "ValidUpto",
            schema: "MultiTenancy",
            table: "Tenants",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTime>(
            "ValidUpto",
            schema: "MultiTenancy",
            table: "Tenants",
            type: "timestamp without time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");
    }
}
