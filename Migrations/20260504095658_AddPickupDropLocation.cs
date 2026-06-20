using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPickupDropLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "Drivers",
                newName: "Username");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DropLocation",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PickupLocation",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "DropLocation",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PickupLocation",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Drivers",
                newName: "PhotoUrl");
        }
    }
}
