namespace ChatroomB_Backend.DTO
{
    public class FriendRequest
    {
        public int ReceivedId { get; set; }
        public int SenderId { get; set; }
        public string? UserName { get; set; }
        public int Status { get; set; }
    }
}
