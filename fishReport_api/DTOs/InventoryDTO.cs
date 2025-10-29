namespace FishReportApi.DTOs
{
    public class SpeciesInventoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class MarketInventoryDTO
    {
        public int Id { get; set; }
        public string MarketName { get; set; }
        public string? Location { get; set; }
        public List<SpeciesInventoryDTO> Species { get; set; } = [];
    }
}