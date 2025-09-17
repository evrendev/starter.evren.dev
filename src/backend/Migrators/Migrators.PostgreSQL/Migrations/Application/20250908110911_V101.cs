#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Migrators.PostgreSQL.Migrations.Application;

/// <inheritdoc />
public partial class V101 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            "Catalog");

        migrationBuilder.EnsureSchema(
            "Auditing");

        migrationBuilder.EnsureSchema(
            "Identity");

        migrationBuilder.CreateTable(
            "Absences",
            schema: "Catalog",
            columns: table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                StartDate = table.Column<DateTime>("timestamp without time zone", nullable: false),
                EndDate = table.Column<DateTime>("timestamp without time zone", nullable: false),
                Description = table.Column<string>("character varying(1000)", maxLength: 1000, nullable: true),
                Location = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                Employee = table.Column<string>("character varying(100)", maxLength: 100, nullable: false),
                CalendarId = table.Column<string>("character varying(100)", maxLength: 100, nullable: false),
                TenantId = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                CreatedBy = table.Column<Guid>("uuid", nullable: false),
                CreatedOn = table.Column<DateTime>("timestamp without time zone", nullable: false),
                LastModifiedBy = table.Column<Guid>("uuid", nullable: false),
                LastModifiedOn = table.Column<DateTime>("timestamp without time zone", nullable: true),
                DeletedOn = table.Column<DateTime>("timestamp without time zone", nullable: true),
                DeletedBy = table.Column<Guid>("uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Absences", x => x.Id);
            });

        migrationBuilder.CreateTable(
            "AuditTrails",
            schema: "Auditing",
            columns: table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                UserId = table.Column<Guid>("uuid", nullable: false),
                Type = table.Column<string>("text", nullable: true),
                TableName = table.Column<string>("text", nullable: true),
                DateTime = table.Column<DateTime>("timestamp without time zone", nullable: false),
                OldValues = table.Column<string>("text", nullable: true),
                NewValues = table.Column<string>("text", nullable: true),
                AffectedColumns = table.Column<string>("text", nullable: true),
                PrimaryKey = table.Column<string>("text", nullable: true),
                TenantId = table.Column<string>("character varying(64)", maxLength: 64, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AuditTrails", x => x.Id);
            });

        migrationBuilder.CreateTable(
            "Brands",
            schema: "Catalog",
            columns: table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                Name = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>("text", nullable: true),
                TenantId = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                CreatedBy = table.Column<Guid>("uuid", nullable: false),
                CreatedOn = table.Column<DateTime>("timestamp without time zone", nullable: false),
                LastModifiedBy = table.Column<Guid>("uuid", nullable: false),
                LastModifiedOn = table.Column<DateTime>("timestamp without time zone", nullable: true),
                DeletedOn = table.Column<DateTime>("timestamp without time zone", nullable: true),
                DeletedBy = table.Column<Guid>("uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Brands", x => x.Id);
            });

        migrationBuilder.CreateTable(
            "Category",
            schema: "Catalog",
            columns: table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                Name = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>("text", nullable: true),
                TenantId = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                CreatedBy = table.Column<Guid>("uuid", nullable: false),
                CreatedOn = table.Column<DateTime>("timestamp without time zone", nullable: false),
                LastModifiedBy = table.Column<Guid>("uuid", nullable: false),
                LastModifiedOn = table.Column<DateTime>("timestamp without time zone", nullable: true),
                DeletedOn = table.Column<DateTime>("timestamp without time zone", nullable: true),
                DeletedBy = table.Column<Guid>("uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Category", x => x.Id);
            });

        migrationBuilder.CreateTable(
            "Roles",
            schema: "Identity",
            columns: table => new
            {
                Id = table.Column<string>("text", nullable: false),
                Description = table.Column<string>("text", nullable: true),
                TenantId = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                Name = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                NormalizedName = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                ConcurrencyStamp = table.Column<string>("text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Roles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            "Users",
            schema: "Identity",
            columns: table => new
            {
                Id = table.Column<string>("text", nullable: false),
                Gender = table.Column<int>("integer", maxLength: 1, nullable: false, defaultValue: 0),
                FirstName = table.Column<string>("character varying(100)", maxLength: 100, nullable: false),
                LastName = table.Column<string>("character varying(100)", maxLength: 100, nullable: false),
                Birthday = table.Column<DateTime>("timestamp without time zone", nullable: true),
                PlaceOfBirth = table.Column<string>("character varying(100)", maxLength: 100, nullable: true),
                ImageUrl = table.Column<string>("text", nullable: true),
                IsActive = table.Column<bool>("boolean", nullable: false),
                RefreshToken = table.Column<string>("text", nullable: true),
                RefreshTokenExpiryTime = table.Column<DateTime>("timestamp without time zone", nullable: false),
                ObjectId = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                Language = table.Column<int>("integer", maxLength: 2, nullable: false, defaultValue: 1),
                TenantId = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                UserName = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                NormalizedUserName = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                Email = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                NormalizedEmail = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                EmailConfirmed = table.Column<bool>("boolean", nullable: false),
                PasswordHash = table.Column<string>("text", nullable: true),
                SecurityStamp = table.Column<string>("text", nullable: true),
                ConcurrencyStamp = table.Column<string>("text", nullable: true),
                PhoneNumber = table.Column<string>("text", nullable: true),
                PhoneNumberConfirmed = table.Column<bool>("boolean", nullable: false),
                TwoFactorEnabled = table.Column<bool>("boolean", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>("timestamp with time zone", nullable: true),
                LockoutEnabled = table.Column<bool>("boolean", nullable: false),
                AccessFailedCount = table.Column<int>("integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            "Products",
            schema: "Catalog",
            columns: table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                Name = table.Column<string>("character varying(1024)", maxLength: 1024, nullable: false),
                Description = table.Column<string>("text", nullable: true),
                Rate = table.Column<decimal>("numeric", nullable: false),
                ImagePath = table.Column<string>("character varying(2048)", maxLength: 2048, nullable: true),
                BrandId = table.Column<Guid>("uuid", nullable: false),
                TenantId = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                CreatedBy = table.Column<Guid>("uuid", nullable: false),
                CreatedOn = table.Column<DateTime>("timestamp without time zone", nullable: false),
                LastModifiedBy = table.Column<Guid>("uuid", nullable: false),
                LastModifiedOn = table.Column<DateTime>("timestamp without time zone", nullable: true),
                DeletedOn = table.Column<DateTime>("timestamp without time zone", nullable: true),
                DeletedBy = table.Column<Guid>("uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Products", x => x.Id);
                table.ForeignKey(
                    "FK_Products_Brands_BrandId",
                    x => x.BrandId,
                    principalSchema: "Catalog",
                    principalTable: "Brands",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "Course",
            schema: "Catalog",
            columns: table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                Title = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                Intrudiction = table.Column<string>("text", nullable: true),
                Description = table.Column<string>("text", nullable: true),
                CategoryId = table.Column<Guid>("uuid", nullable: false),
                Tags = table.Column<string[]>("text[]", nullable: true),
                Image = table.Column<string>("character varying(500)", maxLength: 500, nullable: true),
                Published = table.Column<bool>("boolean", nullable: false),
                Upcoming = table.Column<bool>("boolean", nullable: false),
                Featured = table.Column<bool>("boolean", nullable: false),
                PreviewVideoUrl = table.Column<string>("text", nullable: true),
                Paid = table.Column<bool>("boolean", nullable: false),
                CompletetionCertificate = table.Column<bool>("boolean", nullable: false),
                PaidCertificate = table.Column<bool>("boolean", nullable: false),
                TenantId = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                CreatedBy = table.Column<Guid>("uuid", nullable: false),
                CreatedOn = table.Column<DateTime>("timestamp without time zone", nullable: false),
                LastModifiedBy = table.Column<Guid>("uuid", nullable: false),
                LastModifiedOn = table.Column<DateTime>("timestamp without time zone", nullable: true),
                DeletedOn = table.Column<DateTime>("timestamp without time zone", nullable: true),
                DeletedBy = table.Column<Guid>("uuid", nullable: true)
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
            "RoleClaims",
            schema: "Identity",
            columns: table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                CreatedBy = table.Column<string>("text", nullable: true),
                CreatedOn = table.Column<DateTime>("timestamp without time zone", nullable: false),
                TenantId = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                RoleId = table.Column<string>("text", nullable: false),
                ClaimType = table.Column<string>("text", nullable: true),
                ClaimValue = table.Column<string>("text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RoleClaims", x => x.Id);
                table.ForeignKey(
                    "FK_RoleClaims_Roles_RoleId",
                    x => x.RoleId,
                    principalSchema: "Identity",
                    principalTable: "Roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "UserClaims",
            schema: "Identity",
            columns: table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<string>("text", nullable: false),
                ClaimType = table.Column<string>("text", nullable: true),
                ClaimValue = table.Column<string>("text", nullable: true),
                TenantId = table.Column<string>("character varying(64)", maxLength: 64, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserClaims", x => x.Id);
                table.ForeignKey(
                    "FK_UserClaims_Users_UserId",
                    x => x.UserId,
                    principalSchema: "Identity",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "UserLogins",
            schema: "Identity",
            columns: table => new
            {
                Id = table.Column<string>("text", nullable: false),
                LoginProvider = table.Column<string>("text", nullable: false),
                ProviderKey = table.Column<string>("text", nullable: false),
                ProviderDisplayName = table.Column<string>("text", nullable: true),
                UserId = table.Column<string>("text", nullable: false),
                TenantId = table.Column<string>("character varying(64)", maxLength: 64, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserLogins", x => x.Id);
                table.ForeignKey(
                    "FK_UserLogins_Users_UserId",
                    x => x.UserId,
                    principalSchema: "Identity",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "UserRoles",
            schema: "Identity",
            columns: table => new
            {
                UserId = table.Column<string>("text", nullable: false),
                RoleId = table.Column<string>("text", nullable: false),
                TenantId = table.Column<string>("character varying(64)", maxLength: 64, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                table.ForeignKey(
                    "FK_UserRoles_Roles_RoleId",
                    x => x.RoleId,
                    principalSchema: "Identity",
                    principalTable: "Roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_UserRoles_Users_UserId",
                    x => x.UserId,
                    principalSchema: "Identity",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "UserTokens",
            schema: "Identity",
            columns: table => new
            {
                UserId = table.Column<string>("text", nullable: false),
                LoginProvider = table.Column<string>("text", nullable: false),
                Name = table.Column<string>("text", nullable: false),
                Value = table.Column<string>("text", nullable: true),
                TenantId = table.Column<string>("character varying(64)", maxLength: 64, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey(
                    "FK_UserTokens_Users_UserId",
                    x => x.UserId,
                    principalSchema: "Identity",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "Chapter",
            schema: "Catalog",
            columns: table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                Title = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>("text", nullable: true),
                CourseId = table.Column<Guid>("uuid", nullable: false),
                TenantId = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                CreatedBy = table.Column<Guid>("uuid", nullable: false),
                CreatedOn = table.Column<DateTime>("timestamp without time zone", nullable: false),
                LastModifiedBy = table.Column<Guid>("uuid", nullable: false),
                LastModifiedOn = table.Column<DateTime>("timestamp without time zone", nullable: true),
                DeletedOn = table.Column<DateTime>("timestamp without time zone", nullable: true),
                DeletedBy = table.Column<Guid>("uuid", nullable: true)
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
                UserId = table.Column<string>("text", nullable: false),
                CourseId = table.Column<Guid>("uuid", nullable: false),
                EnrolledAt = table.Column<DateTime>("timestamp without time zone", nullable: false),
                Completed = table.Column<bool>("boolean", nullable: false)
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
                Id = table.Column<Guid>("uuid", nullable: false),
                Title = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>("text", nullable: true),
                Content = table.Column<string>("text", nullable: true),
                Notes = table.Column<string>("text", nullable: true),
                ChapterId = table.Column<Guid>("uuid", nullable: false),
                TenantId = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                CreatedBy = table.Column<Guid>("uuid", nullable: false),
                CreatedOn = table.Column<DateTime>("timestamp without time zone", nullable: false),
                LastModifiedBy = table.Column<Guid>("uuid", nullable: false),
                LastModifiedOn = table.Column<DateTime>("timestamp without time zone", nullable: true),
                DeletedOn = table.Column<DateTime>("timestamp without time zone", nullable: true),
                DeletedBy = table.Column<Guid>("uuid", nullable: true)
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

        migrationBuilder.CreateIndex(
            "IX_Products_BrandId",
            schema: "Catalog",
            table: "Products",
            column: "BrandId");

        migrationBuilder.CreateIndex(
            "IX_RoleClaims_RoleId",
            schema: "Identity",
            table: "RoleClaims",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            "RoleNameIndex",
            schema: "Identity",
            table: "Roles",
            columns: new[] { "NormalizedName", "TenantId" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_UserClaims_UserId",
            schema: "Identity",
            table: "UserClaims",
            column: "UserId");

        migrationBuilder.CreateIndex(
            "IX_UserLogins_LoginProvider_ProviderKey_TenantId",
            schema: "Identity",
            table: "UserLogins",
            columns: new[] { "LoginProvider", "ProviderKey", "TenantId" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_UserLogins_UserId",
            schema: "Identity",
            table: "UserLogins",
            column: "UserId");

        migrationBuilder.CreateIndex(
            "IX_UserRoles_RoleId",
            schema: "Identity",
            table: "UserRoles",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            "EmailIndex",
            schema: "Identity",
            table: "Users",
            column: "NormalizedEmail");

        migrationBuilder.CreateIndex(
            "UserNameIndex",
            schema: "Identity",
            table: "Users",
            columns: new[] { "NormalizedUserName", "TenantId" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "Absences",
            "Catalog");

        migrationBuilder.DropTable(
            "AuditTrails",
            "Auditing");

        migrationBuilder.DropTable(
            "CourseEnrollment",
            "Catalog");

        migrationBuilder.DropTable(
            "Lesson",
            "Catalog");

        migrationBuilder.DropTable(
            "Products",
            "Catalog");

        migrationBuilder.DropTable(
            "RoleClaims",
            "Identity");

        migrationBuilder.DropTable(
            "UserClaims",
            "Identity");

        migrationBuilder.DropTable(
            "UserLogins",
            "Identity");

        migrationBuilder.DropTable(
            "UserRoles",
            "Identity");

        migrationBuilder.DropTable(
            "UserTokens",
            "Identity");

        migrationBuilder.DropTable(
            "Chapter",
            "Catalog");

        migrationBuilder.DropTable(
            "Brands",
            "Catalog");

        migrationBuilder.DropTable(
            "Roles",
            "Identity");

        migrationBuilder.DropTable(
            "Users",
            "Identity");

        migrationBuilder.DropTable(
            "Course",
            "Catalog");

        migrationBuilder.DropTable(
            "Category",
            "Catalog");
    }
}
