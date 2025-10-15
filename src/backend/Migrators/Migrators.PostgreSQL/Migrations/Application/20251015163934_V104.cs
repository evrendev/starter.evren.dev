using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class V104 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                schema: "Catalog",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Image",
                schema: "Catalog",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "Catalog",
                table: "Lessons");

            migrationBuilder.CreateTable(
                name: "Note",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LessonId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    LessonProgressLessonId = table.Column<Guid>(type: "uuid", nullable: true),
                    LessonProgressUserId = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Note", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Note_LessonProgresses_LessonProgressUserId_LessonProgressLe~",
                        columns: x => new { x.LessonProgressUserId, x.LessonProgressLessonId },
                        principalSchema: "Catalog",
                        principalTable: "LessonProgresses",
                        principalColumns: new[] { "UserId", "LessonId" });
                    table.ForeignKey(
                        name: "FK_Note_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalSchema: "Catalog",
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Note_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Note_LessonId",
                schema: "Catalog",
                table: "Note",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Note_LessonProgressUserId_LessonProgressLessonId",
                schema: "Catalog",
                table: "Note",
                columns: new[] { "LessonProgressUserId", "LessonProgressLessonId" });

            migrationBuilder.CreateIndex(
                name: "IX_Note_UserId",
                schema: "Catalog",
                table: "Note",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Note",
                schema: "Catalog");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "Catalog",
                table: "Lessons",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                schema: "Catalog",
                table: "Lessons",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "Catalog",
                table: "Lessons",
                type: "text",
                nullable: true);
        }
    }
}
