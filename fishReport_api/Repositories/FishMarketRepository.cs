using FishReportApi.Data;
using FishReportApi.Models;
using FishReportApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FishReportApi.Repositories
{
    public class FishMarketRepository : GenericRepository<FishMarket>, IFishMarketRepository
    {
        private readonly FishDBContext _context;

        public FishMarketRepository(FishDBContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<IEnumerable<FishMarket>> GetAllAsync()
        {
            return await _context.FishMarkets
                .Include(fm => fm.FishMarketInventory)
                .ThenInclude(fmi => fmi.Species)
                .ToListAsync();
        }

        public override async Task<FishMarket?> GetByIdAsync(int id)
        {
            return await _context.FishMarkets
                .Include(fm => fm.FishMarketInventory)
                .ThenInclude(fmi => fmi.Species)
                .FirstOrDefaultAsync(fm => fm.Id == id);
        }

        public async Task<bool> AddSpeciesToMarketAsync(int marketId, int speciesId)
        {
            var market = await _context.FishMarkets
                .Include(m => m.FishMarketInventory)
                .FirstOrDefaultAsync(m => m.Id == marketId);

            var species = await _context.Species.FindAsync(speciesId);

            if (market == null || species == null)
            {
                return false;
            }

            if (market.FishMarketInventory.Any(fmi => fmi.SpeciesId == speciesId))
            {
                return false;
            }


            _context.FishMarketInventory.Add(new FishMarketInventory
            {
                FishMarketId = marketId,
                SpeciesId = speciesId
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveSpeciesFromMarketAsync(int marketId, int speciesId)
        {
            var inventoryEntry = await _context.FishMarketInventory
                .FirstOrDefaultAsync(fmi => fmi.FishMarketId == marketId && fmi.SpeciesId == speciesId);

            if (inventoryEntry == null)
            {
                return false;
            }

            _context.FishMarketInventory.Remove(inventoryEntry);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}