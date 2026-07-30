using EmployeePerformance.Application.DTOs.ReviewCycle;
using EmployeePerformance.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePerformance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewCycleController : ControllerBase
    {
        private readonly IReviewCycleService _reviewCycleService;

        public ReviewCycleController(IReviewCycleService reviewCycleService)
        {
            _reviewCycleService = reviewCycleService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAll()
        {
            var reviewCycles = await _reviewCycleService.GetAllReviewCyclesAsync();
            return Ok(reviewCycles);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetById(int id)
        {
            var reviewCycle = await _reviewCycleService.GetReviewCycleByIdAsync(id);

            if (reviewCycle == null)
                return NotFound();

            return Ok(reviewCycle);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateReviewCycleDto createReviewCycleDto)
        {
            var createdReviewCycle = await _reviewCycleService.AddReviewCycleAsync(createReviewCycleDto);

            return CreatedAtAction(nameof(GetById), new { id = createdReviewCycle.ReviewCycleId }, new
            {
                Message = "Review Cycle created successfully."
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateReviewCycleDto updateReviewCycleDto)
        {
            await _reviewCycleService.UpdateReviewCycleAsync(id, updateReviewCycleDto);

            return Ok(new
            {
                Message = "Review Cycle updated successfully."
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reviewCycleService.DeleteAsync(id);

            if (!result)
                return NotFound(new
                {
                    Message = "Review Cycle not found."
                });

            return NoContent();
        }
    }
}
