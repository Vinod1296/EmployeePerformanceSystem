using EmployeePerformance.Application.DTOs.Common;
using EmployeePerformance.Application.DTOs.Employee;
using EmployeePerformance.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePerformance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddEmployee(CreateEmployeeDto employeeDto)
        {
            var createdEmployee = await _employeeService.AddEmployeeAsync(employeeDto);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = createdEmployee.EmployeeId }, "Employee Added Successfully");
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeDto employeeDto)
        {
            await _employeeService.UpdateEmployeeAsync(id, employeeDto);
            return Ok("Employee Updated Successfully");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var isDeleted = await _employeeService.DeleteEmployeeAsync(id);

            if (!isDeleted)
            {
                return NotFound("Employee not found");
            }

            return NoContent();
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<PagedResponse<EmployeeDto>>> SearchEmployees([FromQuery] EmployeeSearchDto searchDto)
        {
            var employees = await _employeeService.SearchEmployeesAsync(searchDto);
            return Ok(employees);
        }
    }
}
