using ChatroomB_Backend.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatroomB_Backend.DTO
{
    public class Message
    {
        public int? MessageId { get; set; }

        public string Content { get; set; } = null!;

        public int? UserChatRoomId { get; set; }

        public DateTime? TimeStamp { get; set; }

        public string? ResourceUrl { get; set; }

        public int MessageType { get; set; }

        public bool IsDeleted { get; set; }

        public int? ChatRoomId { get; set; }

        public int? UserId { get; set; }
    }
}
