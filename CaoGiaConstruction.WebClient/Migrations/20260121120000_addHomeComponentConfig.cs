using System;
using CaoGiaConstruction.WebClient.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaoGiaConstruction.WebClient.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260121120000_addHomeComponentConfig")]
    public partial class addHomeComponentConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HomeComponentConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ComponentKey = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Javascript = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeComponentConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HomeComponentConfigs_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HomeComponentConfigs_Users_ModifiedBy",
                        column: x => x.ModifiedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HomeComponentConfigs_CreatedBy",
                table: "HomeComponentConfigs",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_HomeComponentConfigs_ModifiedBy",
                table: "HomeComponentConfigs",
                column: "ModifiedBy");

            migrationBuilder.Sql(@"
INSERT INTO ""HomeComponentConfigs""
    (""Id"", ""Name"", ""ComponentKey"", ""Javascript"", ""SortOrder"", ""Status"", ""IsDeleted"", ""CreatedDate"", ""ModifiedDate"", ""CreatedBy"", ""ModifiedBy"")
VALUES
    ('c1b8d8a7-00ff-4f0b-95f1-6a2e6bded001', 'Slide đầu trang', '_SlideTopHome', NULL, 1, 1, FALSE, '2026-01-21 00:00:00+00', '2026-01-21 00:00:00+00', NULL, NULL),
    ('c1b8d8a7-00ff-4f0b-95f1-6a2e6bded002', 'Máy móc nổi bật', 'vcHotMachine', NULL, 2, 1, FALSE, '2026-01-21 00:00:00+00', '2026-01-21 00:00:00+00', NULL, NULL),
    ('c1b8d8a7-00ff-4f0b-95f1-6a2e6bded003', 'Sản phẩm nổi bật', 'vcHotProduct', NULL, 3, 1, FALSE, '2026-01-21 00:00:00+00', '2026-01-21 00:00:00+00', NULL, NULL),
    ('c1b8d8a7-00ff-4f0b-95f1-6a2e6bded004', 'Giới thiệu', 'vcAboutHome', NULL, 4, 1, FALSE, '2026-01-21 00:00:00+00', '2026-01-21 00:00:00+00', NULL, NULL),
    ('c1b8d8a7-00ff-4f0b-95f1-6a2e6bded005', 'TikTok', '_TiktokEmbedContainer', '$(function () {\n    var container = $(''#tiktok-embed-container'');\n    if (container.length === 0) {\n        return;\n    }\n\n    $.ajax({\n        url: ''/api/tiktok-embed'',\n        method: ''GET'',\n        cache: false,\n        timeout: 10000\n    })\n    .done(function (response) {\n        if (response && response.success && response.html) {\n            container.html(response.html);\n        }\n    })\n    .fail(function (xhr, status, error) {\n        console.log(''Failed to load TikTok embed:'', error);\n    });\n});', 5, 1, FALSE, '2026-01-21 00:00:00+00', '2026-01-21 00:00:00+00', NULL, NULL),
    ('c1b8d8a7-00ff-4f0b-95f1-6a2e6bded006', 'Chi nhánh', 'vcBranchesV2', NULL, 6, 1, FALSE, '2026-01-21 00:00:00+00', '2026-01-21 00:00:00+00', NULL, NULL),
    ('c1b8d8a7-00ff-4f0b-95f1-6a2e6bded007', 'Dịch vụ', 'vcService', NULL, 7, 1, FALSE, '2026-01-21 00:00:00+00', '2026-01-21 00:00:00+00', NULL, NULL),
    ('c1b8d8a7-00ff-4f0b-95f1-6a2e6bded008', 'Blog', 'vcBlogHome', NULL, 8, 1, FALSE, '2026-01-21 00:00:00+00', '2026-01-21 00:00:00+00', NULL, NULL),
    ('c1b8d8a7-00ff-4f0b-95f1-6a2e6bded009', 'Video', 'vcVideo', NULL, 9, 1, FALSE, '2026-01-21 00:00:00+00', '2026-01-21 00:00:00+00', NULL, NULL),
    ('c1b8d8a7-00ff-4f0b-95f1-6a2e6bded010', 'Cảm nhận khách hàng', 'vcFeedback', NULL, 10, 1, FALSE, '2026-01-21 00:00:00+00', '2026-01-21 00:00:00+00', NULL, NULL);
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeComponentConfigs");
        }
    }
}
