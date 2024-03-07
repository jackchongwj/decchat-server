namespace ChatroomB_Backend.DTO
{
    public class RetrieveGroupMember
    {
        public int ChatRoomId { get; set; }
        public int UserChatRoomId { get; set; } 
        public int UserId { get; set; }
        public string? ProfileName { get; set; } 
        public string? ProfilePicture { get; set; } 
        public string? ChatRoomName { get; set; }
        public string? UserPicture { get; set; } 
    }
}
