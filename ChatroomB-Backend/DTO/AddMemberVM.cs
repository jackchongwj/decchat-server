using System.Data;

namespace ChatroomB_Backend.DTO
{
    public class AddMemberVM
    {
        public int ChatRoomId { get; set; }

        public List<int> SelectedUsers { get; set; }
    }
}
