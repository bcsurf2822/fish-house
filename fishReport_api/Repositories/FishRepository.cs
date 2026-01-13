using FishReportApi.Data;
using FishReportApi.Models;
using FishReportApi.Repositories.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FishReportApi.Repositories
{
    public class FishRepository : GenericRepository<Species>, IFishRepository
    {
        private readonly FishDBContext _context;

        public FishRepository(FishDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Species>> GetAllForInventoryAsync()
        {
            return await _context.Species
                .Include(s => s.FishMarketInventory)
                .ThenInclude(fmi => fmi.FishMarket)
                .ToListAsync();
        }

        public async Task<bool> PatchAsync(int id, JsonPatchDocument<Species> patchDoc, ModelStateDictionary modelState)
        {
            Console.WriteLine($"[FishRepository-PatchAsync] Attempting to patch fish {id}");

            var fish = await _context.Species.FindAsync(id);
            if (fish == null)
            {
                Console.WriteLine($"[FishRepository-PatchAsync] Fish {id} not found");
                return false;
            }

            Console.WriteLine($"[FishRepository-PatchAsync] Applying patch document to fish {id}");
            patchDoc.ApplyTo(fish, modelState);

            if (!modelState.IsValid)
            {
                Console.WriteLine($"[FishRepository-PatchAsync] ModelState is invalid after applying patch");
                return false;
            }

            Console.WriteLine($"[FishRepository-PatchAsync] Patch applied successfully. Current price: {fish.Price}");

            try
            {
                var changeCount = await _context.SaveChangesAsync();
                Console.WriteLine($"[FishRepository-PatchAsync] SaveChangesAsync completed. Changes saved: {changeCount}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FishRepository-PatchAsync] ERROR saving changes: {ex.Message}");
                Console.WriteLine($"[FishRepository-PatchAsync] Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}