using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatroomB_Backend.Models
{
    public class UserChatRooms
    {
        [Key]
        public int? UserChatRoomId { get; set; }

        public Users? Users { get; set; }
        [ForeignKey("Users")]
        public int? UserId { get; set; }

        public ChatRooms? ChatRooms { get; set; }
        [ForeignKey("ChatRooms")]
        public int? ChatRoomId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
