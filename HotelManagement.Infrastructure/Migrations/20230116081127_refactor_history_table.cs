using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class refactorhistorytable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Checkin",
                table: "History");

            migrationBuilder.DropColumn(
                name: "Checkout",
                table: "History");

            migrationBuilder.DropColumn(
                name: "CititzenIdentification",
                table: "History");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "History");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "History");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "History");

            migrationBuilder.AddColumn<Guid>(
                name: "BookingId",
                table: "History",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CititzenIdentification",
                table: "Booking",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Booking",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Booking",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentType",
                table: "Booking",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_History_BookingId",
                table: "History",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_History_Booking_BookingId",
                table: "History",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_History_Booking_BookingId",
                table: "History");

            migrationBuilder.DropIndex(
                name: "IX_History_BookingId",
                table: "History");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "History");

            migrationBuilder.DropColumn(
                name: "CititzenIdentification",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Booking");

            migrationBuilder.AddColumn<DateTime>(
                name: "Checkin",
                table: "History",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Checkout",
                table: "History",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CititzenIdentification",
                table: "History",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "History",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "History",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentType",
                table: "History",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
