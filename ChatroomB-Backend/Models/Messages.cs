using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatroomB_Backend.Models
{
    public class Messages
    {
        [Key]
        public int? MessengeId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string Content { get; set; } = null!;

        public UserChatRooms? UserChatRooms { get; set; }
        [ForeignKey("UserChatRooms")]
        public int? UserChatRoomId { get; set; }

        public DateTime? TimeStamp { get; set; }

        [Column(TypeName = "varchar(256)")]
        public string? ResourceUrl { get; set; }

        [Required]
        public int MessageType { get; set; }

        public bool IsDelete { get; set; }
    }
}
