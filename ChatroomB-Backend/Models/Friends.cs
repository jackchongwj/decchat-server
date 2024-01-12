using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatroomB_Backend.Models
{
    public class Friends
    {
        [Key]
        public int? RequestId { get; set; }

        public Users? Sender { get; set; }
        [ForeignKey("Users")]
        public int? SenderId { get; set; }

        public Users? Receiver { get; set; }
        [ForeignKey("Users")]
        public int? ReceiverId { get; set; }

        public int Status { get; set; }
    }
}
