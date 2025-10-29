using FishReportApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FishReportApi.Repositories.Interfaces
{
    public interface IFishRepository
    {
        Task<IEnumerable<Species>> GetAllAsync();
        Task<IEnumerable<Species>> GetAllForInventoryAsync();
        Task<Species?> GetByIdAsync(int id);
        Task<Species> CreateAsync(Species fish);
        Task<bool> UpdateAsync(Species fish);
        Task<bool> PatchAsync(int id, JsonPatchDocument<Species> patchDoc, ModelStateDictionary modelState);
        Task<bool> DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}