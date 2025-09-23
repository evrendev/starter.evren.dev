using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class V101 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Catalog");

            migrationBuilder.EnsureSchema(
                name: "Auditing");

            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.CreateTable(
                name: "Absences",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Location = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Employee = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CalendarId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Absences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditTrails",
                schema: "Auditing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    TableName = table.Column<string>(type: "text", nullable: true),
                    DateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OldValues = table.Column<string>(type: "text", nullable: true),
                    NewValues = table.Column<string>(type: "text", nullable: true),
                    AffectedColumns = table.Column<string>(type: "text", nullable: true),
                    PrimaryKey = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Brands",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Rate = table.Column<decimal>(type: "numeric", nullable: false),
                    ImagePath = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<int>(type: "integer", maxLength: 1, nullable: false, defaultValue: 0),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Birthday = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PlaceOfBirth = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ObjectId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Language = table.Column<int>(type: "integer", maxLength: 2, nullable: false, defaultValue: 1),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Introduction = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tags = table.Column<string[]>(type: "text[]", nullable: true),
                    Image = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Published = table.Column<bool>(type: "boolean", nullable: false),
                    PreviewVideoUrl = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Course_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Catalog",
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "Identity",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                schema: "Identity",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chapter",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chapter_Course_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "Catalog",
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseEnrollment",
                schema: "Catalog",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Completed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseEnrollment", x => new { x.UserId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_CourseEnrollment_Course_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "Catalog",
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseEnrollment_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lesson",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    ChapterId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lesson", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lesson_Chapter_ChapterId",
                        column: x => x.ChapterId,
                        principalSchema: "Catalog",
                        principalTable: "Chapter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chapter_CourseId",
                schema: "Catalog",
                table: "Chapter",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Course_CategoryId",
                schema: "Catalog",
                table: "Course",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollment_CourseId",
                schema: "Catalog",
                table: "CourseEnrollment",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Lesson_ChapterId",
                schema: "Catalog",
                table: "Lesson",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                schema: "Identity",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "Identity",
                table: "Roles",
                columns: new[] { "NormalizedName", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                schema: "Identity",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_LoginProvider_ProviderKey_TenantId",
                schema: "Identity",
                table: "UserLogins",
                columns: new[] { "LoginProvider", "ProviderKey", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                schema: "Identity",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "Identity",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "Identity",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "Identity",
                table: "Users",
                columns: new[] { "NormalizedUserName", "TenantId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Absences",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "AuditTrails",
                schema: "Auditing");

            migrationBuilder.DropTable(
                name: "Brands",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "CourseEnrollment",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "Lesson",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "RoleClaims",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserLogins",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserTokens",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Chapter",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Course",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "Category",
                schema: "Catalog");
        }
    }
}
