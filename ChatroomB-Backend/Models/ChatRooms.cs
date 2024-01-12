using System.ComponentModel.DataAnnotations;

namespace ChatroomB_Backend.Models
{
    public class ChatRooms
    {
        [Key]
        public int chatRoomId { get; set; }

        [Required]
        public string roomName { get; set; } = null!;

        [Required]
        public DateTime createdDate { get; set; }

        [Required]
        public bool roomType { get; set; }

        public string? roomProfilePic { get; set; }

        [Required]
        public int initiatedBy { get; set; }

        [Required]
        public bool? isDelete { get; set; }
    }
}
