using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FishReportApi.Models
{
    public class Login
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(20, ErrorMessage = "Username must be under 20 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, ErrorMessage = "Password must be under 50 characters.")]
        public string Password { get; set; }

        public string? Role { get; set; } = "MarketPolice";
    }
}