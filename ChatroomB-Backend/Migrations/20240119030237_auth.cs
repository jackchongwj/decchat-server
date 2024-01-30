using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatroomB_Backend.Migrations
{
    /// <inheritdoc />
    public partial class auth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ChatRooms");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "Salt");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileName",
                table: "Users",
                type: "nvarchar(15)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)");

            migrationBuilder.AddColumn<string>(
                name: "HashedPassword",
                table: "Users",
                type: "varchar(256)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HashedPassword",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Salt",
                table: "Users",
                newName: "Password");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileName",
                table: "Users",
                type: "nvarchar(15)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ChatRooms",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
