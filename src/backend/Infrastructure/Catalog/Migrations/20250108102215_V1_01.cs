using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvrenDev.Infrastructure.Catalog.Migrations
{
    /// <inheritdoc />
    public partial class V1_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Colour",
                schema: "Catalog",
                table: "TodoLists",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: true,
                defaultValue: "#FFFFFF",
                oldClrType: typeof(string),
                oldType: "nvarchar(7)",
                oldMaxLength: 7);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Colour",
                schema: "Catalog",
                table: "TodoLists",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(7)",
                oldMaxLength: 7,
                oldNullable: true,
                oldDefaultValue: "#FFFFFF");
        }
    }
}
