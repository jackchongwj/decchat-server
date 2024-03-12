using ChatroomB_Backend.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ChatroomB_Backend.DTO
{
    public class ChatlistVM
    {
        public int ChatRoomId { get; set; }
        public int UserChatRoomId { get; set; }
        public int UserId { get; set; }
        public string? ProfileName { get; set; }
        public string? ProfilePicture { get; set; }
        public string? ChatRoomName  { get; set; }
        public bool RoomType { get; set; }
        public DataTable SelectedUsers { get; set; }
        public int InitiatedBy {  get; set; }
        public string? InitiatorProfileName { get; set; }
        public bool IsOnline { get; set; } = false;
    }
}
