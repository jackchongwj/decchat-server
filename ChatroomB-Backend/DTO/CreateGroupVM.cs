namespace ChatroomB_Backend.DTO
{
    public class CreateGroupVM
    {
        public string RoomName { get; set; }
        public List<int> SelectedUsers { get; set; }
        public int InitiatedBy { get; set; }
        public int? UserId { get; set; }

    }
}
