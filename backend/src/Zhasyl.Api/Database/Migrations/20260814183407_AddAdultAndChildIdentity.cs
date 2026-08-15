using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zhasyl.Api.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAdultAndChildIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "adult_accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    PreferredLocale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adult_accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "child_profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdultAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    LearningLocale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_child_profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_child_profiles_adult_accounts_AdultAccountId",
                        column: x => x.AdultAccountId,
                        principalTable: "adult_accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "oauth_identities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdultAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    ProviderSubject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ProviderEmail = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    LinkedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oauth_identities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_oauth_identities_adult_accounts_AdultAccountId",
                        column: x => x.AdultAccountId,
                        principalTable: "adult_accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "child_device_sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChildProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DeviceName = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_child_device_sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_child_device_sessions_child_profiles_ChildProfileId",
                        column: x => x.ChildProfileId,
                        principalTable: "child_profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "device_pairing_codes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChildProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    CodeHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UsedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_pairing_codes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_device_pairing_codes_child_profiles_ChildProfileId",
                        column: x => x.ChildProfileId,
                        principalTable: "child_profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_adult_accounts_Email",
                table: "adult_accounts",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_child_device_sessions_ChildProfileId_ExpiresAt",
                table: "child_device_sessions",
                columns: new[] { "ChildProfileId", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_child_device_sessions_TokenHash",
                table: "child_device_sessions",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_child_profiles_AdultAccountId_DisplayName",
                table: "child_profiles",
                columns: new[] { "AdultAccountId", "DisplayName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_device_pairing_codes_ChildProfileId_ExpiresAt",
                table: "device_pairing_codes",
                columns: new[] { "ChildProfileId", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_device_pairing_codes_CodeHash",
                table: "device_pairing_codes",
                column: "CodeHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_oauth_identities_AdultAccountId",
                table: "oauth_identities",
                column: "AdultAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_oauth_identities_Provider_ProviderSubject",
                table: "oauth_identities",
                columns: new[] { "Provider", "ProviderSubject" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "child_device_sessions");

            migrationBuilder.DropTable(
                name: "device_pairing_codes");

            migrationBuilder.DropTable(
                name: "oauth_identities");

            migrationBuilder.DropTable(
                name: "child_profiles");

            migrationBuilder.DropTable(
                name: "adult_accounts");
        }
    }
}
