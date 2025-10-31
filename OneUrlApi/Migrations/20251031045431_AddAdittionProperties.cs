using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneUrlApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAdittionProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InteractCount",
                table: "UrlRecords",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCollectingInteractCount",
                table: "Settings",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InteractCount",
                table: "UrlRecords");

            migrationBuilder.DropColumn(
                name: "IsCollectingInteractCount",
                table: "Settings");
        }
    }
}
