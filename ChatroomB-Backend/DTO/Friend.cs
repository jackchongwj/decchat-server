namespace ChatroomB_Backend.DTO
{
    public class FriendRequest
    {
        public int ReceiverId { get; set; }
        public int SenderId { get; set; }
        public int Status { get; set; }
    }

    public class DeleteFriendRequest 
    {
        public int ChatRoomId { get; set; }
        public int UserId1 { get; set; }
        public int UserId2 { get; set; }
    }
}
