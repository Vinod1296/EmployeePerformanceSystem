using BCrypt.Net;
using EmployeePerformance.Application.DTOs.Auth;
using EmployeePerformance.Application.Configuration;
using EmployeePerformance.Application.Exceptions;
using EmployeePerformance.Application.Interfaces;
using EmployeePerformance.Domain.Entities;
using Microsoft.Extensions.Options;

namespace EmployeePerformance.Application.Services
{
    /// <summary>
    /// Provides authentication-related application use cases.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly JwtSettings _jwtSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// </summary>
        /// <param name="authRepository">Authentication repository.</param>
        /// <param name="jwtTokenGenerator">JWT token generator.</param>
        /// <param name="jwtOptions">JWT settings.</param>
        public AuthService(
            IAuthRepository authRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IOptions<JwtSettings> jwtOptions,
            ICurrentUserContext currentUserContext)
        {
            _authRepository = authRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _jwtSettings = jwtOptions.Value;
            _currentUserContext = currentUserContext;
        }

        /// <inheritdoc />
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var user = await _authRepository.GetUserByUsernameAsync(request.Username);

            if (user is null)
            {
                throw new UnauthorizedException();
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedException();
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedException("Your account has been deactivated. Please contact an administrator.");
            }

            var token = _jwtTokenGenerator.GenerateToken(user);
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes);

            return new LoginResponseDto
            {
                UserId = user.UserId,
                EmployeeId = user.EmployeeId,
                Username = user.Username,
                Role = user.Role,
                Token = token,
                Expiration = expiration,
                Message = "Login successful."
            };
        }

        /// <inheritdoc />
        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            var employee = await _authRepository.GetEmployeeByIdAsync(request.EmployeeId);
            if (employee is null)
            {
                throw new ArgumentException("Employee not found.");
            }

            if (!employee.IsActive)
            {
                throw new InvalidOperationException("Employee is inactive.");
            }

            if (await _authRepository.EmployeeAlreadyRegisteredAsync(request.EmployeeId))
            {
                throw new EmployeeAlreadyRegisteredException();
            }

            if (await _authRepository.UsernameExistsAsync(request.Username))
            {
                throw new InvalidOperationException("Username already exists.");
            }

            var user = new User
            {
                EmployeeId = request.EmployeeId,
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "Employee",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _authRepository.CreateUserAsync(user);

            return new RegisterResponseDto
            {
                UserId = createdUser.UserId,
                EmployeeId = createdUser.EmployeeId,
                Username = createdUser.Username,
                Role = createdUser.Role,
                Message = "User registered successfully."
            };
        }

        /// <inheritdoc />
        public async Task<ChangePasswordResponseDto> ChangePasswordAsync(ChangePasswordDto request)
        {
            var currentUserId = _currentUserContext.UserId;
            if (!currentUserId.HasValue)
            {
                throw new UnauthorizedException();
            }

            ArgumentNullException.ThrowIfNull(request);

            var user = await _authRepository.GetUserByIdAsync(currentUserId.Value);
            if (user is null)
            {
                throw new UserNotFoundException();
            }

            if (!user.IsActive)
            {
                throw new AccountInactiveException();
            }

            if (!string.Equals(request.NewPassword, request.ConfirmPassword, StringComparison.Ordinal))
            {
                throw new PasswordConfirmationMismatchException();
            }

            if (!IsPasswordStrongEnough(request.NewPassword))
            {
                throw new PasswordStrengthException();
            }

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                throw new CurrentPasswordIncorrectException();
            }

            if (string.Equals(request.NewPassword, request.CurrentPassword, StringComparison.Ordinal))
            {
                throw new NewPasswordMatchesCurrentPasswordException();
            }

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _authRepository.ChangePasswordAsync(user.UserId, newPasswordHash);

            return new ChangePasswordResponseDto
            {
                Message = "Password changed successfully."
            };
        }

        private static bool IsPasswordStrongEnough(string password)
        {
            return !string.IsNullOrEmpty(password) && password.Length >= 6 && password.Length <= 100;
        }
    }
}
