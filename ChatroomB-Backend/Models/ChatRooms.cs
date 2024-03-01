using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatroomB_Backend.Models
{
    public class ChatRooms
    {
        [Key]
        public int ChatRoomId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string RoomName { get; set; } = null!;

        [Required]
        public bool RoomType { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? RoomProfilePic { get; set; }

        [Required]
        public int InitiatedBy { get; set; }

        [Required]
        public bool? IsDeleted { get; set; }
    }
}
