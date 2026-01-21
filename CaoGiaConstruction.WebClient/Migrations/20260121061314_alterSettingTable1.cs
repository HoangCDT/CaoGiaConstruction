using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaoGiaConstruction.WebClient.Migrations
{
    /// <inheritdoc />
    public partial class alterSettingTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeaderMenuHoverColor",
                table: "Settings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubMenuBorderTopColor",
                table: "Settings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubMenuTextColor",
                table: "Settings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeaderMenuHoverColor",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "SubMenuBorderTopColor",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "SubMenuTextColor",
                table: "Settings");
        }
    }
}
