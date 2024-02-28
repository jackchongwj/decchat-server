using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatroomB_Backend.Models
{
    public class Messages
    {
        [Key]
        public int? MessageId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string Content { get; set; } = null!;

        public UserChatRooms? UserChatRooms { get; set; }
        [ForeignKey("UserChatRooms")]
        public int? UserChatRoomId { get; set; }

        public DateTime? TimeStamp { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? ResourceUrl { get; set; }

        public bool IsDeleted { get; set; }
    }
}
