using AutoMapper;
using FishReportApi.Data;
using FishReportApi.DTOs;
using FishReportApi.Models;
using FishReportApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FishReportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FishController : ControllerBase
    {
        private readonly IFishRepository _repository;
        private readonly IMapper _mapper;
        public FishController(IFishRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        //GET ALL FISH
        [HttpGet("getAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllFish()
        {
            var fishList = await _repository.GetAllAsync();
            return Ok(fishList);
        }

        [HttpGet("fishInventory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllSpeciesInventory()
        {
            var speciesList = await _repository.GetAllForInventoryAsync();
            var speciesDTOs = _mapper.Map<IEnumerable<SpeciesInventoryDTO>>(speciesList);
            return Ok(speciesDTOs);
        }


        //GET FISH
        [HttpGet("getById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFishById(int id)
        {
            var fish = await _repository.GetByIdAsync(id);
            if (fish == null) return NotFound();
            return Ok(fish);
        }

        //POST FISH
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateFish([FromBody] Species fish)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _repository.CreateAsync(fish);
            await _repository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFishById), new { id = fish.Id }, fish);
        }

        //PATCH | Update Partial
        [Authorize]
        [HttpPatch("updatePartial/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchFish(int id, [FromBody] JsonPatchDocument<Species> patchDoc)
        {
            var success = await _repository.PatchAsync(id, patchDoc, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!success)
                return NotFound();

            await _repository.SaveChangesAsync();
            return NoContent();
        }


        //DELETE | Fish
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteFish(int id)
        {
            var success = await _repository.DeleteAsync(id);
            if (!success) return NotFound();

            await _repository.SaveChangesAsync();
            return NoContent();
        }
    }
}