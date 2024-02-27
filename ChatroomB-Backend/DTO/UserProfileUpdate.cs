namespace ChatroomB_Backend.DTO
{
    public class UserProfileUpdate
    {
        public int UserId { get; set; }
        public string? ProfileName { get; set; }
        public string? ProfilePicture { get; set; }
        public List<int>? FriendIds { get; set; }
    }
}
