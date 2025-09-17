#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.MSSQL.Migrations.Application;

/// <inheritdoc />
public partial class V1_01 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            "LastName",
            schema: "Identity",
            table: "Users",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            "FirstName",
            schema: "Identity",
            table: "Users",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AddColumn<DateTime>(
            "Birthday",
            schema: "Identity",
            table: "Users",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            "PlaceOfBirth",
            schema: "Identity",
            table: "Users",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: true);

        migrationBuilder.CreateTable(
            "Category",
            schema: "Catalog",
            columns: table => new
            {
                Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                Name = table.Column<string>("nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>("nvarchar(max)", nullable: true),
                TenantId = table.Column<string>("nvarchar(64)", maxLength: 64, nullable: false),
                CreatedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                CreatedOn = table.Column<DateTime>("datetime2", nullable: false),
                LastModifiedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                LastModifiedOn = table.Column<DateTime>("datetime2", nullable: true),
                DeletedOn = table.Column<DateTime>("datetime2", nullable: true),
                DeletedBy = table.Column<Guid>("uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Category", x => x.Id);
            });

        migrationBuilder.CreateTable(
            "Course",
            schema: "Catalog",
            columns: table => new
            {
                Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                Title = table.Column<string>("nvarchar(256)", maxLength: 256, nullable: false),
                Intrudiction = table.Column<string>("nvarchar(max)", nullable: true),
                Description = table.Column<string>("nvarchar(max)", nullable: true),
                CategoryId = table.Column<Guid>("uniqueidentifier", nullable: false),
                Tags = table.Column<string>("nvarchar(max)", nullable: true),
                Image = table.Column<string>("nvarchar(500)", maxLength: 500, nullable: true),
                Published = table.Column<bool>("bit", nullable: false),
                Upcoming = table.Column<bool>("bit", nullable: false),
                Featured = table.Column<bool>("bit", nullable: false),
                PreviewVideoUrl = table.Column<string>("nvarchar(max)", nullable: true),
                Paid = table.Column<bool>("bit", nullable: false),
                CompletetionCertificate = table.Column<bool>("bit", nullable: false),
                PaidCertificate = table.Column<bool>("bit", nullable: false),
                TenantId = table.Column<string>("nvarchar(64)", maxLength: 64, nullable: false),
                CreatedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                CreatedOn = table.Column<DateTime>("datetime2", nullable: false),
                LastModifiedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                LastModifiedOn = table.Column<DateTime>("datetime2", nullable: true),
                DeletedOn = table.Column<DateTime>("datetime2", nullable: true),
                DeletedBy = table.Column<Guid>("uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Course", x => x.Id);
                table.ForeignKey(
                    "FK_Course_Category_CategoryId",
                    x => x.CategoryId,
                    principalSchema: "Catalog",
                    principalTable: "Category",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "Chapter",
            schema: "Catalog",
            columns: table => new
            {
                Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                Title = table.Column<string>("nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>("nvarchar(max)", nullable: true),
                CourseId = table.Column<Guid>("uniqueidentifier", nullable: false),
                TenantId = table.Column<string>("nvarchar(64)", maxLength: 64, nullable: false),
                CreatedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                CreatedOn = table.Column<DateTime>("datetime2", nullable: false),
                LastModifiedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                LastModifiedOn = table.Column<DateTime>("datetime2", nullable: true),
                DeletedOn = table.Column<DateTime>("datetime2", nullable: true),
                DeletedBy = table.Column<Guid>("uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Chapter", x => x.Id);
                table.ForeignKey(
                    "FK_Chapter_Course_CourseId",
                    x => x.CourseId,
                    principalSchema: "Catalog",
                    principalTable: "Course",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "CourseEnrollment",
            schema: "Catalog",
            columns: table => new
            {
                UserId = table.Column<string>("nvarchar(450)", nullable: false),
                CourseId = table.Column<Guid>("uniqueidentifier", nullable: false),
                EnrolledAt = table.Column<DateTime>("datetime2", nullable: false),
                Completed = table.Column<bool>("bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CourseEnrollment", x => new { x.UserId, x.CourseId });
                table.ForeignKey(
                    "FK_CourseEnrollment_Course_CourseId",
                    x => x.CourseId,
                    principalSchema: "Catalog",
                    principalTable: "Course",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_CourseEnrollment_Users_UserId",
                    x => x.UserId,
                    principalSchema: "Identity",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "Lesson",
            schema: "Catalog",
            columns: table => new
            {
                Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                Title = table.Column<string>("nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>("nvarchar(max)", nullable: true),
                Content = table.Column<string>("nvarchar(max)", nullable: true),
                Notes = table.Column<string>("nvarchar(max)", nullable: true),
                ChapterId = table.Column<Guid>("uniqueidentifier", nullable: false),
                TenantId = table.Column<string>("nvarchar(64)", maxLength: 64, nullable: false),
                CreatedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                CreatedOn = table.Column<DateTime>("datetime2", nullable: false),
                LastModifiedBy = table.Column<Guid>("uniqueidentifier", nullable: false),
                LastModifiedOn = table.Column<DateTime>("datetime2", nullable: true),
                DeletedOn = table.Column<DateTime>("datetime2", nullable: true),
                DeletedBy = table.Column<Guid>("uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Lesson", x => x.Id);
                table.ForeignKey(
                    "FK_Lesson_Chapter_ChapterId",
                    x => x.ChapterId,
                    principalSchema: "Catalog",
                    principalTable: "Chapter",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "IX_Chapter_CourseId",
            schema: "Catalog",
            table: "Chapter",
            column: "CourseId");

        migrationBuilder.CreateIndex(
            "IX_Course_CategoryId",
            schema: "Catalog",
            table: "Course",
            column: "CategoryId");

        migrationBuilder.CreateIndex(
            "IX_CourseEnrollment_CourseId",
            schema: "Catalog",
            table: "CourseEnrollment",
            column: "CourseId");

        migrationBuilder.CreateIndex(
            "IX_Lesson_ChapterId",
            schema: "Catalog",
            table: "Lesson",
            column: "ChapterId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "CourseEnrollment",
            "Catalog");

        migrationBuilder.DropTable(
            "Lesson",
            "Catalog");

        migrationBuilder.DropTable(
            "Chapter",
            "Catalog");

        migrationBuilder.DropTable(
            "Course",
            "Catalog");

        migrationBuilder.DropTable(
            "Category",
            "Catalog");

        migrationBuilder.DropColumn(
            "Birthday",
            schema: "Identity",
            table: "Users");

        migrationBuilder.DropColumn(
            "PlaceOfBirth",
            schema: "Identity",
            table: "Users");

        migrationBuilder.AlterColumn<string>(
            "LastName",
            schema: "Identity",
            table: "Users",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(100)",
            oldMaxLength: 100);

        migrationBuilder.AlterColumn<string>(
            "FirstName",
            schema: "Identity",
            table: "Users",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(100)",
            oldMaxLength: 100);
    }
}
