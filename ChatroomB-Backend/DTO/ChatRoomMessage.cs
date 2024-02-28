using ChatroomB_Backend.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatroomB_Backend.DTO
{
    public class ChatRoomMessage
    {
        public int? MessageId { get; set; }

        public string Content { get; set; } = null!;

        public int? UserChatRoomId { get; set; }

        public DateTime? TimeStamp { get; set; }

        public string? ResourceUrl { get; set; }

        public bool IsDeleted { get; set; }

        public int? ChatRoomId { get; set; }

        public int? UserId { get; set; }

        public string? ProfileName { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
