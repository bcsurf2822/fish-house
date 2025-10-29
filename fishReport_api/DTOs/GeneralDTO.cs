namespace FishReportApi.DTOs
{
    public class SpeciesDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Habitat { get; set; }
        public decimal Price { get; set; }
        public int Length { get; set; }
        public int Lifespan { get; set; }
        public int? Population { get; set; }
    }
    public class FishMarketDTO
    {
        public int Id { get; set; }
        public string MarketName { get; set; }
        public string? Location { get; set; }
        public List<SpeciesDTO>? Species { get; set; } = new();
    }
}