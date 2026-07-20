using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class AddPageCollapse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Lessons/LessonPages are dropped and Pages rebuilt empty (no data migration —
            // task explicitly authorized this, no prod data exists yet). Existing Notes
            // point at old LessonPage ids that won't exist in the new Pages table, so they'd
            // violate the rebuilt FK; clearing them is the same "start fresh" call already
            // made for the rest of the catalog tree.
            migrationBuilder.Sql("DELETE FROM \"Catalog\".\"Note\";");

            migrationBuilder.DropForeignKey(
                name: "FK_Note_LessonPages_LessonPageId",
                schema: "Catalog",
                table: "Note");

            migrationBuilder.DropTable(
                name: "LessonPageProgresses",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "LessonProgresses",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "LessonPages",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "Lessons",
                schema: "Catalog");

            migrationBuilder.RenameColumn(
                name: "LessonPageId",
                schema: "Catalog",
                table: "Note",
                newName: "PageId");

            migrationBuilder.RenameIndex(
                name: "IX_Note_LessonPageId",
                schema: "Catalog",
                table: "Note",
                newName: "IX_Note_PageId");

            migrationBuilder.CreateTable(
                name: "ChapterProgresses",
                schema: "Catalog",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ChapterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PercentComplete = table.Column<int>(type: "integer", nullable: false),
                    LastVisitedPageId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterProgresses", x => new { x.UserId, x.ChapterId });
                    table.ForeignKey(
                        name: "FK_ChapterProgresses_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalSchema: "Catalog",
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChapterProgresses_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pages",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    MediaUrl = table.Column<string>(type: "text", nullable: true),
                    ChapterId = table.Column<Guid>(type: "uuid", nullable: false),
                    NeedsReview = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsImported = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pages_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalSchema: "Catalog",
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageProgresses",
                schema: "Catalog",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    PageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Completed = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastVisitedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageProgresses", x => new { x.UserId, x.PageId });
                    table.ForeignKey(
                        name: "FK_PageProgresses_Pages_PageId",
                        column: x => x.PageId,
                        principalSchema: "Catalog",
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PageProgresses_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChapterProgresses_ChapterId",
                schema: "Catalog",
                table: "ChapterProgresses",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_PageProgresses_PageId",
                schema: "Catalog",
                table: "PageProgresses",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_ChapterId",
                schema: "Catalog",
                table: "Pages",
                column: "ChapterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Note_Pages_PageId",
                schema: "Catalog",
                table: "Note",
                column: "PageId",
                principalSchema: "Catalog",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_Pages_PageId",
                schema: "Catalog",
                table: "Note");

            migrationBuilder.DropTable(
                name: "ChapterProgresses",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "PageProgresses",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "Pages",
                schema: "Catalog");

            migrationBuilder.RenameColumn(
                name: "PageId",
                schema: "Catalog",
                table: "Note",
                newName: "LessonPageId");

            migrationBuilder.RenameIndex(
                name: "IX_Note_PageId",
                schema: "Catalog",
                table: "Note",
                newName: "IX_Note_LessonPageId");

            migrationBuilder.CreateTable(
                name: "Lessons",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChapterId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lessons_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalSchema: "Catalog",
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonPages",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LessonId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsImported = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MediaUrl = table.Column<string>(type: "text", nullable: true),
                    NeedsReview = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
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
                name: "LessonProgresses",
                schema: "Catalog",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LessonId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastVisitedPageId = table.Column<Guid>(type: "uuid", nullable: true),
                    PercentComplete = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonProgresses", x => new { x.UserId, x.LessonId });
                    table.ForeignKey(
                        name: "FK_LessonProgresses_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalSchema: "Catalog",
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonProgresses_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
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

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgresses_LessonId",
                schema: "Catalog",
                table: "LessonProgresses",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_ChapterId",
                schema: "Catalog",
                table: "Lessons",
                column: "ChapterId");

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
    }
}
