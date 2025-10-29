using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FishReportApi.Models
{
  public class Species
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Species name required")]
    [StringLength(100)]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Please provide habitat")]
    [StringLength(100)]
    public string? Habitat { get; set; }

    [Range(0.0, 500.0, ErrorMessage = "Length must be between 1 and 500 cm.")]
    public int Length { get; set; }

    public int? Population { get; set; }
    public int Lifespan { get; set; }

    [Range(0.01, 200.00, ErrorMessage = "Price must be between $0.01 and $200.00.")]
    public decimal Price { get; set; }

    public List<FishMarketInventory> FishMarketInventory { get; set; } = [];
  }
}