using ChatroomB_Backend.Models;

namespace ChatroomB_Backend.DTO
{
    public class FileMessage
    {
        public ChatRoomMessage? Message { get; set; }
        public byte[]? FileByte { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
    }
}
