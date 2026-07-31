using EmployeePerformance.Application.DTOs.Common;
using EmployeePerformance.Application.DTOs.PerformanceReview;
using EmployeePerformance.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeePerformance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PerformanceReviewController : ControllerBase
    {
        private readonly IPerformanceReviewService _performanceReviewService;

        public PerformanceReviewController(IPerformanceReviewService performanceReviewService)
        {
            _performanceReviewService = performanceReviewService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> GetAll()
        {
            var currentUser = GetCurrentUserContext();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var performanceReviews = await _performanceReviewService.GetAllAsync(currentUser);
            return Ok(performanceReviews);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> GetById(int id)
        {
            var currentUser = GetCurrentUserContext();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var performanceReview = await _performanceReviewService.GetByIdAsync(id, currentUser);
            if (performanceReview == null)
            {
                return NotFound();
            }
            return Ok(performanceReview);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Add(CreatePerformanceReviewDto dto)
        {
            var createdPerformanceReview = await _performanceReviewService.AddAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = createdPerformanceReview.performanceReviewId }, "Performance Review Created Successfully.");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, UpdatePerformanceReviewDto dto)
        {
            await _performanceReviewService.UpdateAsync(id, dto);

            return Ok("Performance Review Updated Successfully.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _performanceReviewService.DeleteAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        private CurrentUserContextDto? GetCurrentUserContext()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var employeeIdClaim = User.FindFirstValue("EmployeeId");

            if (string.IsNullOrWhiteSpace(role) || !int.TryParse(employeeIdClaim, out var employeeId))
            {
                return null;
            }

            return new CurrentUserContextDto
            {
                EmployeeId = employeeId,
                Role = role
            };
        }
    }
}
