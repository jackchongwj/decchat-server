namespace ChatroomB_Backend.DTO
{
    public class GroupProfileUpdate
    {
        public int ChatRoomId { get; set; }
        public string? GroupName { get; set; }
        public string? GroupPicture { get; set; }
        public List<int>? MemberIds { get; set; }
    }
}
