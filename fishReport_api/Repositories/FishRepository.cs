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
            var fish = await _context.Species.FindAsync(id);
            if (fish == null)
            {
                return false;
            }
            patchDoc.ApplyTo(fish, modelState);


            if (!modelState.IsValid)
            {
                return false;
            }


            await _context.SaveChangesAsync();
            return true;
        }
    }
}