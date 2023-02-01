using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatebookingtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Room_Booking_BookingId",
                table: "Room");

            migrationBuilder.DropIndex(
                name: "IX_Room_BookingId",
                table: "Room");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Room");

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "Booking",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_RoomId",
                table: "Booking",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Room_RoomId",
                table: "Booking",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Room_RoomId",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_RoomId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Booking");

            migrationBuilder.AddColumn<Guid>(
                name: "BookingId",
                table: "Room",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Room_BookingId",
                table: "Room",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Room_Booking_BookingId",
                table: "Room",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id");
        }
    }
}
