using EmployeePerformance.Application.DTOs.Rating;
using EmployeePerformance.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePerformance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RatingController : ControllerBase
    {
        private readonly IRatingServices _ratingServices;

        public RatingController(IRatingServices ratingServices)
        {
            _ratingServices = ratingServices;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAll()
        {
            var ratings = await _ratingServices.GetAllAsync();
            return Ok(ratings);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetById(int id)
        {
            var rating = await _ratingServices.GetByIdAsync(id);
            if (rating == null)
                return NotFound();
            return Ok(rating);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Add(CreateRatingDto dto)
        {
            var createdRating = await _ratingServices.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdRating.RatingId }, "Rating Created Successfully.");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Update(int id, UpdateRatingDto dto)
        {
            await _ratingServices.UpdateAsync(id, dto);

            return Ok("Rating Updated Successfully.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _ratingServices.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
