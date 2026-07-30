using System.Security.Claims;
using EmployeePerformance.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace EmployeePerformance.API.Services
{
    public class HttpContextCurrentUserContext : ICurrentUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextCurrentUserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var userIdValue = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? user?.FindFirstValue("UserId");

                if (int.TryParse(userIdValue, out var userId))
                {
                    return userId;
                }

                return null;
            }
        }
    }
}
