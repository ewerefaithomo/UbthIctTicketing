using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UbthIctTicketing.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathToIncident : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Incidents");
        }
    }
}
