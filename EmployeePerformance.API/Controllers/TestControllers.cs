using EmployeePerformance.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePerformance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class TestControllers : ControllerBase
{
    private readonly EmployeePerformanceDbContext _context;

    public TestControllers(EmployeePerformanceDbContext context)
    {
        _context = context;
    }

    [HttpGet("db-check")]
    public IActionResult CheckDb()
    {
        var employeesCount = _context.Employees.Count();
        return Ok(new { Message = "Database connected!", EmployeeCount = employeesCount });
    }
}
