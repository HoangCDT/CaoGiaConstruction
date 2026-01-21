using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaoGiaConstruction.WebClient.Migrations
{
    /// <inheritdoc />
    public partial class alterSettingTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HeaderBackgroundColor",
                table: "Settings",
                newName: "SecondaryColor");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryColor",
                table: "Settings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryColor",
                table: "Settings");

            migrationBuilder.RenameColumn(
                name: "SecondaryColor",
                table: "Settings",
                newName: "HeaderBackgroundColor");
        }
    }
}
