using System;
using CaoGiaConstruction.WebClient.Context;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaoGiaConstruction.WebClient.Migrations
{
    [Microsoft.EntityFrameworkCore.Migrations.Migration("20260203100000_AddSlideCategoryBannerAboutHome")]
    public partial class AddSlideCategoryBannerAboutHome : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        protected override void Up(Microsoft.EntityFrameworkCore.Migrations.MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO ""SlideCategories""
                    (""Id"", ""Title"", ""Description"", ""Content"", ""Code"", ""Avatar"", ""SortOrder"", ""Status"", ""IsDeleted"", ""CreatedDate"", ""ModifiedDate"", ""CreatedBy"", ""ModifiedBy"")
                SELECT
                    'a1b2c3d4-e5f6-4789-a012-bannerabouthome01'::uuid,
                    'Banner Giới thiệu Home',
                    'Banner giới thiệu hiển thị trong slider trang chủ',
                    NULL,
                    'HOME_BANNER_ABOUT_HOME',
                    NULL,
                    7,
                    1,
                    FALSE,
                    NOW() AT TIME ZONE 'UTC',
                    NOW() AT TIME ZONE 'UTC',
                    NULL,
                    NULL
                WHERE NOT EXISTS (
                    SELECT 1 FROM ""SlideCategories"" WHERE ""Code"" = 'HOME_BANNER_ABOUT_HOME'
                );
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM ""SlideCategories"" WHERE ""Code"" = 'HOME_BANNER_ABOUT_HOME';");
        }
    }
}
