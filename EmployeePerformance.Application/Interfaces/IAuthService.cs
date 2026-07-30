using EmployeePerformance.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePerformance.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);

        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);

        Task<ChangePasswordResponseDto> ChangePasswordAsync(ChangePasswordDto request);
    }
}
