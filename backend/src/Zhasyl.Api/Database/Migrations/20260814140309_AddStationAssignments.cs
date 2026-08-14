using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zhasyl.Api.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddStationAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "station_assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_station_assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_station_assignments_missions_MissionId",
                        column: x => x.MissionId,
                        principalTable: "missions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "station_assignment_revisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StationAssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Locale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Objective = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    EstimatedMinutes = table.Column<int>(type: "integer", nullable: false),
                    BodyMdx = table.Column<string>(type: "text", nullable: false),
                    ContentHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PublishedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_station_assignment_revisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_station_assignment_revisions_station_assignments_StationAss~",
                        column: x => x.StationAssignmentId,
                        principalTable: "station_assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_station_assignment_revisions_StationAssignmentId_Locale",
                table: "station_assignment_revisions",
                columns: new[] { "StationAssignmentId", "Locale" },
                unique: true,
                filter: "\"IsCurrent\"");

            migrationBuilder.CreateIndex(
                name: "IX_station_assignment_revisions_StationAssignmentId_Locale_Ver~",
                table: "station_assignment_revisions",
                columns: new[] { "StationAssignmentId", "Locale", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_station_assignments_MissionId_Order",
                table: "station_assignments",
                columns: new[] { "MissionId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_station_assignments_MissionId_Slug",
                table: "station_assignments",
                columns: new[] { "MissionId", "Slug" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "station_assignment_revisions");

            migrationBuilder.DropTable(
                name: "station_assignments");
        }
    }
}
