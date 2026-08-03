using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitZoneGymScheduler.Migrations
{
    /// <inheritdoc />
    public partial class AddedMeasurementUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeightUnit",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WeightUnit",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeightUnit",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "WeightUnit",
                table: "Members");
        }
    }
}
