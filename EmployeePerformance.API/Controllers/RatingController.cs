using EmployeePerformance.Application.DTOs.Rating;
using EmployeePerformance.Application.DTOs.Common;
using EmployeePerformance.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            var currentUser = GetCurrentUserContext();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var createdRating = await _ratingServices.AddAsync(dto, currentUser);
            return CreatedAtAction(nameof(GetById), new { id = createdRating.RatingId }, "Rating Created Successfully.");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Update(int id, UpdateRatingDto dto)
        {
            var currentUser = GetCurrentUserContext();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            await _ratingServices.UpdateAsync(id, dto, currentUser);

            return Ok("Rating Updated Successfully.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = GetCurrentUserContext();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var result = await _ratingServices.DeleteAsync(id, currentUser);

            if (!result)
                return NotFound();

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
