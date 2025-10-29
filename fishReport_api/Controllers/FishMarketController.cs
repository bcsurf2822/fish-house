using AutoMapper;
using FishReportApi.DTOs;
using FishReportApi.Models;
using FishReportApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FishReportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FishMarketController : ControllerBase
    {
        private readonly IFishMarketRepository _repository;
        private readonly IMapper _mapper;


        public FishMarketController(IFishMarketRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET
        [HttpGet("getAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var markets = await _repository.GetAllAsync();
            var marketDTOs = _mapper.Map<IEnumerable<FishMarketDTO>>(markets);
            return Ok(marketDTOs);
        }

        // GET
        [HttpGet("marketid/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            var market = await _repository.GetByIdAsync(id);
            if (market == null) return NotFound();

            var marketDTO = _mapper.Map<FishMarketDTO>(market);

            return Ok(marketDTO);
        }

        //GET ALL INVENTORYDTO
        [HttpGet("inventory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMarketSummary()
        {
            var markets = await _repository.GetAllAsync();
            var summaryDTOs = _mapper.Map<IEnumerable<MarketInventoryDTO>>(markets);
            return Ok(summaryDTOs);
        }

        // POST
        [Authorize]
        [HttpPost("createnew")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] MarketInventoryDTO marketDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Create FishMarket object from DTO
            var market = new FishMarket
            {
                MarketName = marketDTO.MarketName,
                Location = marketDTO.Location,
                FishMarketInventory = new List<FishMarketInventory>()
            };

            await _repository.CreateAsync(market);
            await _repository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = market.Id }, marketDTO);
        }


        //ADD TO INVENTORY
        [Authorize]
        [HttpPost("addtoinventory/{marketId}/{speciesId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddSpeciesToMarket(int marketId, int speciesId)
        {
            var success = await _repository.AddSpeciesToMarketAsync(marketId, speciesId);

            if (!success)
                return NotFound("Market or Species not found, or Species already exists in Market Inventory.");

            return Ok($"Species {speciesId} successfully added to Market {marketId}");
        }

        // PUT
        [HttpPut("update/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] FishMarket updatedMarket)
        {
            if (id != updatedMarket.Id)
                return BadRequest("ID mismatch");

            var success = await _repository.UpdateAsync(updatedMarket);
            if (!success) return NotFound();

            await _repository.SaveChangesAsync();
            return NoContent();
        }

        // PATCH
        [Authorize]
        [HttpPatch("updatepartial/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<FishMarket> patchDoc)
        {
            var market = await _repository.GetByIdAsync(id);
            if (market == null) return NotFound();

            patchDoc.ApplyTo(market, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _repository.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("deletefrominventory/{marketId}/{speciesId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveSpeciesFromMarket(int marketId, int speciesId)
        {
            var success = await _repository.RemoveSpeciesFromMarketAsync(marketId, speciesId);

            if (!success)
            {
                return NotFound("Market or species not found, or species is not in the market inventory.");
            }

            return NoContent();
        }

        // DELETE
        [Authorize]
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "Unauthorized. Please log in to perform this action." });
            }

            var success = await _repository.DeleteAsync(id);
            if (!success) return NotFound();

            await _repository.SaveChangesAsync();
            return NoContent();
        }
    }
}