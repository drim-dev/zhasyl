using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zhasyl.Api.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "stations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Slug = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "laboratories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Slug = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_laboratories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_laboratories_stations_StationId",
                        column: x => x.StationId,
                        principalTable: "stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "station_translations",
                columns: table => new
                {
                    StationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Locale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Location = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    Briefing = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_station_translations", x => new { x.StationId, x.Locale });
                    table.ForeignKey(
                        name: "FK_station_translations_stations_StationId",
                        column: x => x.StationId,
                        principalTable: "stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "laboratory_translations",
                columns: table => new
                {
                    LaboratoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Locale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Purpose = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Specialist = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_laboratory_translations", x => new { x.LaboratoryId, x.Locale });
                    table.ForeignKey(
                        name: "FK_laboratory_translations_laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalTable: "laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "missions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LaboratoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_missions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_missions_laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalTable: "laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mission_revisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Locale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Problem = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    BodyMdx = table.Column<string>(type: "text", nullable: false),
                    ContentHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PublishedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mission_revisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mission_revisions_missions_MissionId",
                        column: x => x.MissionId,
                        principalTable: "missions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_laboratories_StationId_Order",
                table: "laboratories",
                columns: new[] { "StationId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_laboratories_StationId_Slug",
                table: "laboratories",
                columns: new[] { "StationId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mission_revisions_MissionId_Locale",
                table: "mission_revisions",
                columns: new[] { "MissionId", "Locale" },
                unique: true,
                filter: "\"IsCurrent\"");

            migrationBuilder.CreateIndex(
                name: "IX_mission_revisions_MissionId_Locale_Version",
                table: "mission_revisions",
                columns: new[] { "MissionId", "Locale", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_missions_LaboratoryId_Order",
                table: "missions",
                columns: new[] { "LaboratoryId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_missions_LaboratoryId_Slug",
                table: "missions",
                columns: new[] { "LaboratoryId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stations_Slug",
                table: "stations",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "laboratory_translations");

            migrationBuilder.DropTable(
                name: "mission_revisions");

            migrationBuilder.DropTable(
                name: "station_translations");

            migrationBuilder.DropTable(
                name: "missions");

            migrationBuilder.DropTable(
                name: "laboratories");

            migrationBuilder.DropTable(
                name: "stations");
        }
    }
}
