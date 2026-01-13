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
                Console.WriteLine($"[FishMarketRepository-AddSpeciesToMarketAsync] Market or species not found. MarketId: {marketId}, SpeciesId: {speciesId}");
                return false;
            }

            if (market.FishMarketInventory.Any(fmi => fmi.SpeciesId == speciesId))
            {
                Console.WriteLine($"[FishMarketRepository-AddSpeciesToMarketAsync] Species {speciesId} already exists in market {marketId}");
                return false;
            }

            var newInventoryItem = new FishMarketInventory
            {
                FishMarketId = marketId,
                SpeciesId = speciesId
            };

            Console.WriteLine($"[FishMarketRepository-AddSpeciesToMarketAsync] Adding species {speciesId} to market {marketId}");
            _context.FishMarketInventory.Add(newInventoryItem);

            try
            {
                var changeCount = await _context.SaveChangesAsync();
                Console.WriteLine($"[FishMarketRepository-AddSpeciesToMarketAsync] SaveChangesAsync completed. Changes saved: {changeCount}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FishMarketRepository-AddSpeciesToMarketAsync] ERROR saving changes: {ex.Message}");
                Console.WriteLine($"[FishMarketRepository-AddSpeciesToMarketAsync] Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> RemoveSpeciesFromMarketAsync(int marketId, int speciesId)
        {
            Console.WriteLine($"[FishMarketRepository-RemoveSpeciesFromMarketAsync] Attempting to remove species {speciesId} from market {marketId}");

            var inventoryEntry = await _context.FishMarketInventory
                .FirstOrDefaultAsync(fmi => fmi.FishMarketId == marketId && fmi.SpeciesId == speciesId);

            if (inventoryEntry == null)
            {
                Console.WriteLine($"[FishMarketRepository-RemoveSpeciesFromMarketAsync] Inventory entry not found for market {marketId}, species {speciesId}");
                return false;
            }

            Console.WriteLine($"[FishMarketRepository-RemoveSpeciesFromMarketAsync] Removing inventory entry");
            _context.FishMarketInventory.Remove(inventoryEntry);

            try
            {
                var changeCount = await _context.SaveChangesAsync();
                Console.WriteLine($"[FishMarketRepository-RemoveSpeciesFromMarketAsync] SaveChangesAsync completed. Changes saved: {changeCount}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FishMarketRepository-RemoveSpeciesFromMarketAsync] ERROR saving changes: {ex.Message}");
                Console.WriteLine($"[FishMarketRepository-RemoveSpeciesFromMarketAsync] Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}