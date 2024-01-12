using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatroomB_Backend.Models
{
    public class RefreshToken
    {
        [Key]
        public Guid TokenId { get; set; }

        public Users? Users { get; set; }
        [ForeignKey("Users")]
        public int userId { get; set; }

        [Required]
        public string TokenHash { get; set; } = null!;

        [Required]
        public DateTime expiredDateTime { get; set; }

        public bool isDelete { get; set; }
    }
}
