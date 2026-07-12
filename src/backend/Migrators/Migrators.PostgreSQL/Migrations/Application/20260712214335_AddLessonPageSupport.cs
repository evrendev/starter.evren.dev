using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class AddLessonPageSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_LessonProgresses_LessonProgressUserId_LessonProgressLe~",
                schema: "Catalog",
                table: "Note");

            migrationBuilder.DropForeignKey(
                name: "FK_Note_Lessons_LessonId",
                schema: "Catalog",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Note_LessonProgressUserId_LessonProgressLessonId",
                schema: "Catalog",
                table: "Note");

            migrationBuilder.DropColumn(
                name: "LessonProgressLessonId",
                schema: "Catalog",
                table: "Note");

            migrationBuilder.DropColumn(
                name: "LessonProgressUserId",
                schema: "Catalog",
                table: "Note");

            migrationBuilder.DropColumn(
                name: "Content",
                schema: "Catalog",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Completed",
                schema: "Catalog",
                table: "LessonProgresses");

            migrationBuilder.RenameColumn(
                name: "LessonId",
                schema: "Catalog",
                table: "Note",
                newName: "LessonPageId");

            migrationBuilder.RenameIndex(
                name: "IX_Note_LessonId",
                schema: "Catalog",
                table: "Note",
                newName: "IX_Note_LessonPageId");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "Catalog",
                table: "Lessons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                schema: "Catalog",
                table: "LessonProgresses",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<Guid>(
                name: "LastVisitedPageId",
                schema: "Catalog",
                table: "LessonProgresses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PercentComplete",
                schema: "Catalog",
                table: "LessonProgresses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "Catalog",
                table: "LessonProgresses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "Catalog",
                table: "Chapters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LessonPages",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    MediaUrl = table.Column<string>(type: "text", nullable: true),
                    LessonId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonPages_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalSchema: "Catalog",
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonPageProgresses",
                schema: "Catalog",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LessonPageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Completed = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastVisitedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonPageProgresses", x => new { x.UserId, x.LessonPageId });
                    table.ForeignKey(
                        name: "FK_LessonPageProgresses_LessonPages_LessonPageId",
                        column: x => x.LessonPageId,
                        principalSchema: "Catalog",
                        principalTable: "LessonPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonPageProgresses_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonPageProgresses_LessonPageId",
                schema: "Catalog",
                table: "LessonPageProgresses",
                column: "LessonPageId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPages_LessonId",
                schema: "Catalog",
                table: "LessonPages",
                column: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Note_LessonPages_LessonPageId",
                schema: "Catalog",
                table: "Note",
                column: "LessonPageId",
                principalSchema: "Catalog",
                principalTable: "LessonPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_LessonPages_LessonPageId",
                schema: "Catalog",
                table: "Note");

            migrationBuilder.DropTable(
                name: "LessonPageProgresses",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "LessonPages",
                schema: "Catalog");

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "Catalog",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "LastVisitedPageId",
                schema: "Catalog",
                table: "LessonProgresses");

            migrationBuilder.DropColumn(
                name: "PercentComplete",
                schema: "Catalog",
                table: "LessonProgresses");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Catalog",
                table: "LessonProgresses");

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "Catalog",
                table: "Chapters");

            migrationBuilder.RenameColumn(
                name: "LessonPageId",
                schema: "Catalog",
                table: "Note",
                newName: "LessonId");

            migrationBuilder.RenameIndex(
                name: "IX_Note_LessonPageId",
                schema: "Catalog",
                table: "Note",
                newName: "IX_Note_LessonId");

            migrationBuilder.AddColumn<Guid>(
                name: "LessonProgressLessonId",
                schema: "Catalog",
                table: "Note",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LessonProgressUserId",
                schema: "Catalog",
                table: "Note",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                schema: "Catalog",
                table: "Lessons",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                schema: "Catalog",
                table: "LessonProgresses",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                schema: "Catalog",
                table: "LessonProgresses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Note_LessonProgressUserId_LessonProgressLessonId",
                schema: "Catalog",
                table: "Note",
                columns: new[] { "LessonProgressUserId", "LessonProgressLessonId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Note_LessonProgresses_LessonProgressUserId_LessonProgressLe~",
                schema: "Catalog",
                table: "Note",
                columns: new[] { "LessonProgressUserId", "LessonProgressLessonId" },
                principalSchema: "Catalog",
                principalTable: "LessonProgresses",
                principalColumns: new[] { "UserId", "LessonId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Note_Lessons_LessonId",
                schema: "Catalog",
                table: "Note",
                column: "LessonId",
                principalSchema: "Catalog",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
