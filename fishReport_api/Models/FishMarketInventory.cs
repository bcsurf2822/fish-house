using System.ComponentModel.DataAnnotations.Schema;

namespace FishReportApi.Models
{
    public class FishMarketInventory
    {
        public int FishMarketId { get; set; }
        public FishMarket? FishMarket { get; set; }
        public int SpeciesId { get; set; }
        public Species? Species { get; set; }
    }
}