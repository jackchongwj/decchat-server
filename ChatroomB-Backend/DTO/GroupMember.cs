using System.ComponentModel.DataAnnotations.Schema;

namespace ChatroomB_Backend.DTO
{
    public class GroupMember
    {
        public int ChatRoomId { get; set; }
        public int UserId { get; set; }
        public string ProfileName { get; set; } = null!;
        public string ProfilePicture { get; set; }
    }
}
