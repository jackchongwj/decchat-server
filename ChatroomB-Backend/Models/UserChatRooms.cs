using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatroomB_Backend.Models
{
    public class UserChatRooms
    {
        [Key]
        public int userChatRoomId { get; set; }

        public Users? Users { get; set; }
        [ForeignKey("Users")]
        public int userId { get; set; }

        public ChatRooms? ChatRooms { get; set; }
        [ForeignKey("ChatRooms")]
        public int chatRoomId { get; set; }

        public bool isDelete { get; set; }
    }
}
