using ChatroomB_Backend.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore; 

namespace ChatroomB_Backend.DTO
{
    public class ChatlistVM
    {
        public string? ProfileName { get; set; }
        public string? ProfilePicture { get; set; }
        public string? RoomName { get; set; }
        public string? RoomProfilePic { get; set; }
        public bool RoomType { get; set; }
        public int ChatRoomId { get; set; }

    }
}
