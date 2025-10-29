using FishReportApi.Models;

namespace FishReportApi.Repositories.Interfaces
{
    public interface IFishMarketRepository
    {
        Task<IEnumerable<FishMarket>> GetAllAsync();
        Task<FishMarket?> GetByIdAsync(int id);
        Task<FishMarket> CreateAsync(FishMarket market);
        Task<bool> UpdateAsync(FishMarket market);
        Task<bool> AddSpeciesToMarketAsync(int marketId, int speciesId);
        Task<bool> RemoveSpeciesFromMarketAsync(int marketId, int speciesId);
        Task<bool> DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}