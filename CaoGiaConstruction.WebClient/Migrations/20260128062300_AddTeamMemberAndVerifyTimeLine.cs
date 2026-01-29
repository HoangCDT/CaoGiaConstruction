using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaoGiaConstruction.WebClient.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamMemberAndVerifyTimeLine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "TimeLines",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TeamMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Position = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Avatar = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Quote = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsFounder = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMembers_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamMembers_Users_ModifiedBy",
                        column: x => x.ModifiedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_CreatedBy",
                table: "TeamMembers",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_ModifiedBy",
                table: "TeamMembers",
                column: "ModifiedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "TimeLines");
        }
    }
}
