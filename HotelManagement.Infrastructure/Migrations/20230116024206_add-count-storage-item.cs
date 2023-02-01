using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addcountstorageitem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "ItemStorage",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AvailableCount",
                table: "ItemStorage",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnavailableCount",
                table: "ItemStorage",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableCount",
                table: "ItemStorage");

            migrationBuilder.DropColumn(
                name: "UnavailableCount",
                table: "ItemStorage");

            migrationBuilder.AlterColumn<string>(
                name: "Quantity",
                table: "ItemStorage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
