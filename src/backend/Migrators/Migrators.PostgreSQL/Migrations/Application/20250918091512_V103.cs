using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class V103 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletetionCertificate",
                schema: "Catalog",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "Featured",
                schema: "Catalog",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "Paid",
                schema: "Catalog",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "PaidCertificate",
                schema: "Catalog",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "Upcoming",
                schema: "Catalog",
                table: "Course");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CompletetionCertificate",
                schema: "Catalog",
                table: "Course",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Featured",
                schema: "Catalog",
                table: "Course",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Paid",
                schema: "Catalog",
                table: "Course",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PaidCertificate",
                schema: "Catalog",
                table: "Course",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Upcoming",
                schema: "Catalog",
                table: "Course",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
