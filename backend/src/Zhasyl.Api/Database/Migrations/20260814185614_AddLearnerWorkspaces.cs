using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zhasyl.Api.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLearnerWorkspaces : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "learner_workspaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChildProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    StationAssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentVersion = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_learner_workspaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_learner_workspaces_child_profiles_ChildProfileId",
                        column: x => x.ChildProfileId,
                        principalTable: "child_profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_learner_workspaces_station_assignment_revisions_AssignmentR~",
                        column: x => x.AssignmentRevisionId,
                        principalTable: "station_assignment_revisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_learner_workspaces_station_assignments_StationAssignmentId",
                        column: x => x.StationAssignmentId,
                        principalTable: "station_assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "workspace_snapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LearnerWorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    BlobName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ContentHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ByteLength = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workspace_snapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_workspace_snapshots_learner_workspaces_LearnerWorkspaceId",
                        column: x => x.LearnerWorkspaceId,
                        principalTable: "learner_workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_learner_workspaces_AssignmentRevisionId",
                table: "learner_workspaces",
                column: "AssignmentRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_learner_workspaces_ChildProfileId_StationAssignmentId",
                table: "learner_workspaces",
                columns: new[] { "ChildProfileId", "StationAssignmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_learner_workspaces_StationAssignmentId",
                table: "learner_workspaces",
                column: "StationAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_workspace_snapshots_LearnerWorkspaceId_Version",
                table: "workspace_snapshots",
                columns: new[] { "LearnerWorkspaceId", "Version" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "workspace_snapshots");

            migrationBuilder.DropTable(
                name: "learner_workspaces");
        }
    }
}
