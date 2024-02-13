using ChatroomB_Backend.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatroomB_Backend.DTO
{
    public class ChatRoomMessage
    {
        public int? MessageId { get; set; }
        public string? Content { get; set; } = null!;
        public DateTime? TimeStamp { get; set; }

        public string? ResourceUrl { get; set; }
        public int? MessageType { get; set; }
        public int? UserChatRoomId { get; set; }
        public int? UserId { get; set; }
    }
}
