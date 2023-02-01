using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class renametable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_User_CustomerId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_User_StaffId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_History_User_CustomerId",
                table: "History");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemStorage_User_CreatedByUserId",
                table: "ItemStorage");

            migrationBuilder.DropForeignKey(
                name: "FK_RightMapRole_Role_RoleId",
                table: "RightMapRole");

            migrationBuilder.DropForeignKey(
                name: "FK_RightMapUser_User_UserId",
                table: "RightMapUser");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMapRole_Role_RoleId",
                table: "UserMapRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMapRole_User_UserId",
                table: "UserMapRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role",
                table: "Role");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "ht_User");

            migrationBuilder.RenameTable(
                name: "Role",
                newName: "ht_Role");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ht_User",
                table: "ht_User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ht_Role",
                table: "ht_Role",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_ht_User_CustomerId",
                table: "Booking",
                column: "CustomerId",
                principalTable: "ht_User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_ht_User_StaffId",
                table: "Booking",
                column: "StaffId",
                principalTable: "ht_User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_History_ht_User_CustomerId",
                table: "History",
                column: "CustomerId",
                principalTable: "ht_User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemStorage_ht_User_CreatedByUserId",
                table: "ItemStorage",
                column: "CreatedByUserId",
                principalTable: "ht_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RightMapRole_ht_Role_RoleId",
                table: "RightMapRole",
                column: "RoleId",
                principalTable: "ht_Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RightMapUser_ht_User_UserId",
                table: "RightMapUser",
                column: "UserId",
                principalTable: "ht_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMapRole_ht_Role_RoleId",
                table: "UserMapRole",
                column: "RoleId",
                principalTable: "ht_Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMapRole_ht_User_UserId",
                table: "UserMapRole",
                column: "UserId",
                principalTable: "ht_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_ht_User_CustomerId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_ht_User_StaffId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_History_ht_User_CustomerId",
                table: "History");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemStorage_ht_User_CreatedByUserId",
                table: "ItemStorage");

            migrationBuilder.DropForeignKey(
                name: "FK_RightMapRole_ht_Role_RoleId",
                table: "RightMapRole");

            migrationBuilder.DropForeignKey(
                name: "FK_RightMapUser_ht_User_UserId",
                table: "RightMapUser");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMapRole_ht_Role_RoleId",
                table: "UserMapRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMapRole_ht_User_UserId",
                table: "UserMapRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ht_User",
                table: "ht_User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ht_Role",
                table: "ht_Role");

            migrationBuilder.RenameTable(
                name: "ht_User",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "ht_Role",
                newName: "Role");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role",
                table: "Role",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_User_CustomerId",
                table: "Booking",
                column: "CustomerId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_User_StaffId",
                table: "Booking",
                column: "StaffId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_History_User_CustomerId",
                table: "History",
                column: "CustomerId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemStorage_User_CreatedByUserId",
                table: "ItemStorage",
                column: "CreatedByUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RightMapRole_Role_RoleId",
                table: "RightMapRole",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RightMapUser_User_UserId",
                table: "RightMapUser",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMapRole_Role_RoleId",
                table: "UserMapRole",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMapRole_User_UserId",
                table: "UserMapRole",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
