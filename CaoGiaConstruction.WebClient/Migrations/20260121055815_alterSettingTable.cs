using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaoGiaConstruction.WebClient.Migrations
{
    /// <inheritdoc />
    public partial class alterSettingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FooterSubTextColor",
                table: "Settings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeaderMenuTextColor",
                table: "Settings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeaderMenuTextSelectedColor",
                table: "Settings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FooterSubTextColor",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "HeaderMenuTextColor",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "HeaderMenuTextSelectedColor",
                table: "Settings");
        }
    }
}
