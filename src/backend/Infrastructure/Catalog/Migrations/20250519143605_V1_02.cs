using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvrenDev.Infrastructure.Catalog.Migrations
{
    /// <inheritdoc />
    public partial class V1_02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Catalog",
                table: "TodoLists");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Catalog",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Catalog",
                table: "Absences");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Catalog",
                table: "TodoLists",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Catalog",
                table: "TodoItems",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Catalog",
                table: "Absences",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }
    }
}
