using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantBooker.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddIsAvailableToTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Tables",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Tables");
        }
    }
}
