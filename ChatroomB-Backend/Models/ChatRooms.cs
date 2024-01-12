using System.ComponentModel.DataAnnotations;

namespace ChatroomB_Backend.Models
{
    public class ChatRooms
    {
        [Key]
        public int ChatRoomId { get; set; }

        [Required]
        public string RoomName { get; set; } = null!;

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public bool RoomType { get; set; }

        public string? RoomProfilePic { get; set; }

        [Required]
        public int InitiatedBy { get; set; }

        [Required]
        public bool? IsDelete { get; set; }
    }
}
