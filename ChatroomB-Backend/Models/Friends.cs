using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatroomB_Backend.Models
{
    public class Friends
    {
        [Key]
        public int? requestId { get; set; }

        public Users? Sender { get; set; }
        [ForeignKey("Sender")]
        public int senderId { get; set; }

        public Users? Receiver { get; set; }
        [ForeignKey("Receiver")]
        public int ReceiverId { get; set; }

        public int status { get; set; }
    }
}
