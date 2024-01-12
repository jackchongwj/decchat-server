using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatroomB_Backend.Models
{
    public class Messages
    {
        [Key]
        public int messengeId { get; set; }

        [Required]
        public string content { get; set; } = null!;

        public UserChatRooms? UserChatRooms { get; set; }
        [ForeignKey("UserChatRooms")]
        public int userChatRoomId { get; set; }

        public DateTime timeStamp { get; set; }

        public string? resourceUrl { get; set; }

        [Required]
        public int messageType { get; set; }

        public bool isDelete { get; set; }
    }
}
