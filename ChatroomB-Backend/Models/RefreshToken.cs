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
        public int? UserId { get; set; }

        [Required]
        [Column(TypeName = "varchar(256)")]
        public string TokenHash { get; set; } = null!;

        [Required]
        public DateTime? ExpiredDateTime { get; set; }

        public bool IsDeleted { get; set; }
    }
}
