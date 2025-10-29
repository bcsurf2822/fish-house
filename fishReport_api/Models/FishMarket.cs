using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FishReportApi.Models
{
    public class FishMarket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Market name is required.")]
        [StringLength(100, ErrorMessage = "Market name must be under 100 characters.")]
        public string MarketName { get; set; }

        [StringLength(200, ErrorMessage = "Location must be under 200 characters.")]
        public string? Location { get; set; }

        public List<FishMarketInventory> FishMarketInventory { get; set; } = [];
    }
}