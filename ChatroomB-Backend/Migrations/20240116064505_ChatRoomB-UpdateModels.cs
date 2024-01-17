using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatroomB_Backend.Migrations
{
    /// <inheritdoc />
    public partial class ChatRoomBUpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDelete",
                table: "Users",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsDelete",
                table: "UserChatRooms",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsDelete",
                table: "RefreshToken",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsDelete",
                table: "Messages",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "MessengeId",
                table: "Messages",
                newName: "MessageId");

            migrationBuilder.RenameColumn(
                name: "IsDelete",
                table: "ChatRooms",
                newName: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Users",
                newName: "IsDelete");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "UserChatRooms",
                newName: "IsDelete");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "RefreshToken",
                newName: "IsDelete");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Messages",
                newName: "IsDelete");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "Messages",
                newName: "MessengeId");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "ChatRooms",
                newName: "IsDelete");
        }
    }
}
