using System.ComponentModel.DataAnnotations;

namespace test.api.Models
{
    public class UserAccount
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [StringLength(150)]
        public string Fullname { get; set; } = string.Empty;
    }
}
