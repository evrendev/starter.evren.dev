using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvrenDev.Infrastructure.Catalog.Migrations
{
    /// <inheritdoc />
    public partial class V1_03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                schema: "Catalog",
                table: "Absences");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                schema: "Catalog",
                table: "Absences",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Location",
                schema: "Catalog",
                table: "Absences",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "Catalog",
                table: "Absences",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
