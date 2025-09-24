using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class V102 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapter_Course_CourseId",
                schema: "Catalog",
                table: "Chapter");

            migrationBuilder.DropForeignKey(
                name: "FK_Course_Category_CategoryId",
                schema: "Catalog",
                table: "Course");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollment_Course_CourseId",
                schema: "Catalog",
                table: "CourseEnrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollment_Users_UserId",
                schema: "Catalog",
                table: "CourseEnrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_Lesson_Chapter_ChapterId",
                schema: "Catalog",
                table: "Lesson");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lesson",
                schema: "Catalog",
                table: "Lesson");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseEnrollment",
                schema: "Catalog",
                table: "CourseEnrollment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Course",
                schema: "Catalog",
                table: "Course");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chapter",
                schema: "Catalog",
                table: "Chapter");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                schema: "Catalog",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "Completed",
                schema: "Catalog",
                table: "CourseEnrollment");

            migrationBuilder.RenameTable(
                name: "Lesson",
                schema: "Catalog",
                newName: "Lessons",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "CourseEnrollment",
                schema: "Catalog",
                newName: "CourseEnrollments",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "Course",
                schema: "Catalog",
                newName: "Courses",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "Chapter",
                schema: "Catalog",
                newName: "Chapters",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "Category",
                schema: "Catalog",
                newName: "Categories",
                newSchema: "Catalog");

            migrationBuilder.RenameIndex(
                name: "IX_Lesson_ChapterId",
                schema: "Catalog",
                table: "Lessons",
                newName: "IX_Lessons_ChapterId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseEnrollment_CourseId",
                schema: "Catalog",
                table: "CourseEnrollments",
                newName: "IX_CourseEnrollments_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Course_CategoryId",
                schema: "Catalog",
                table: "Courses",
                newName: "IX_Courses_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Chapter_CourseId",
                schema: "Catalog",
                table: "Chapters",
                newName: "IX_Chapters_CourseId");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                schema: "Catalog",
                table: "Lessons",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePaid",
                schema: "Catalog",
                table: "CourseEnrollments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lessons",
                schema: "Catalog",
                table: "Lessons",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseEnrollments",
                schema: "Catalog",
                table: "CourseEnrollments",
                columns: new[] { "UserId", "CourseId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Courses",
                schema: "Catalog",
                table: "Courses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chapters",
                schema: "Catalog",
                table: "Chapters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                schema: "Catalog",
                table: "Categories",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "LessonProgresses",
                schema: "Catalog",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LessonId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Completed = table.Column<bool>(type: "boolean", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgresses_LessonId",
                schema: "Catalog",
                table: "LessonProgresses",
                column: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Courses_CourseId",
                schema: "Catalog",
                table: "Chapters",
                column: "CourseId",
                principalSchema: "Catalog",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollments_Courses_CourseId",
                schema: "Catalog",
                table: "CourseEnrollments",
                column: "CourseId",
                principalSchema: "Catalog",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollments_Users_UserId",
                schema: "Catalog",
                table: "CourseEnrollments",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Categories_CategoryId",
                schema: "Catalog",
                table: "Courses",
                column: "CategoryId",
                principalSchema: "Catalog",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Chapters_ChapterId",
                schema: "Catalog",
                table: "Lessons",
                column: "ChapterId",
                principalSchema: "Catalog",
                principalTable: "Chapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Courses_CourseId",
                schema: "Catalog",
                table: "Chapters");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollments_Courses_CourseId",
                schema: "Catalog",
                table: "CourseEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollments_Users_UserId",
                schema: "Catalog",
                table: "CourseEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Categories_CategoryId",
                schema: "Catalog",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Chapters_ChapterId",
                schema: "Catalog",
                table: "Lessons");

            migrationBuilder.DropTable(
                name: "LessonProgresses",
                schema: "Catalog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lessons",
                schema: "Catalog",
                table: "Lessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Courses",
                schema: "Catalog",
                table: "Courses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseEnrollments",
                schema: "Catalog",
                table: "CourseEnrollments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chapters",
                schema: "Catalog",
                table: "Chapters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                schema: "Catalog",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Image",
                schema: "Catalog",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "PricePaid",
                schema: "Catalog",
                table: "CourseEnrollments");

            migrationBuilder.RenameTable(
                name: "Lessons",
                schema: "Catalog",
                newName: "Lesson",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "Courses",
                schema: "Catalog",
                newName: "Course",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "CourseEnrollments",
                schema: "Catalog",
                newName: "CourseEnrollment",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "Chapters",
                schema: "Catalog",
                newName: "Chapter",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "Categories",
                schema: "Catalog",
                newName: "Category",
                newSchema: "Catalog");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_ChapterId",
                schema: "Catalog",
                table: "Lesson",
                newName: "IX_Lesson_ChapterId");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_CategoryId",
                schema: "Catalog",
                table: "Course",
                newName: "IX_Course_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseEnrollments_CourseId",
                schema: "Catalog",
                table: "CourseEnrollment",
                newName: "IX_CourseEnrollment_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Chapters_CourseId",
                schema: "Catalog",
                table: "Chapter",
                newName: "IX_Chapter_CourseId");

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                schema: "Catalog",
                table: "CourseEnrollment",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lesson",
                schema: "Catalog",
                table: "Lesson",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Course",
                schema: "Catalog",
                table: "Course",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseEnrollment",
                schema: "Catalog",
                table: "CourseEnrollment",
                columns: new[] { "UserId", "CourseId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chapter",
                schema: "Catalog",
                table: "Chapter",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                schema: "Catalog",
                table: "Category",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapter_Course_CourseId",
                schema: "Catalog",
                table: "Chapter",
                column: "CourseId",
                principalSchema: "Catalog",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Course_Category_CategoryId",
                schema: "Catalog",
                table: "Course",
                column: "CategoryId",
                principalSchema: "Catalog",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollment_Course_CourseId",
                schema: "Catalog",
                table: "CourseEnrollment",
                column: "CourseId",
                principalSchema: "Catalog",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollment_Users_UserId",
                schema: "Catalog",
                table: "CourseEnrollment",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lesson_Chapter_ChapterId",
                schema: "Catalog",
                table: "Lesson",
                column: "ChapterId",
                principalSchema: "Catalog",
                principalTable: "Chapter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
