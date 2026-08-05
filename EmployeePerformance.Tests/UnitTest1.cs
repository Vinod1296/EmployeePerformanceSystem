using EmployeePerformance.Application.DTOs.Auth;
using EmployeePerformance.Application.DTOs.Common;
using EmployeePerformance.Application.DTOs.PerformanceReview;
using EmployeePerformance.Application.Interfaces;
using EmployeePerformance.Application.Services;
using EmployeePerformance.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace EmployeePerformance.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_ForcesEmployeeRole()
    {
        var authRepository = new Mock<IAuthRepository>();
        var tokenGenerator = new Mock<IJwtTokenGenerator>();
        var currentUserContext = new Mock<ICurrentUserContext>();
        var jwtSettings = Options.Create(new Application.Configuration.JwtSettings
        {
            Issuer = "issuer",
            Audience = "audience",
            Key = "super-secret-test-key-super-secret-test-key",
            ExpireMinutes = 60
        });

        authRepository.Setup(x => x.UsernameExistsAsync("rahul"))
            .ReturnsAsync(false);
        authRepository.Setup(x => x.GetEmployeeByIdAsync(10))
            .ReturnsAsync(new Employee
            {
                EmployeeId = 10,
                IsActive = true
            });
        authRepository.Setup(x => x.EmployeeExistsAsync(10))
            .ReturnsAsync(true);
        authRepository.Setup(x => x.EmployeeAlreadyRegisteredAsync(10))
            .ReturnsAsync(false);

        User? createdUser = null;
        authRepository.Setup(x => x.CreateUserAsync(It.IsAny<User>()))
            .ReturnsAsync((User user) =>
            {
                createdUser = user;
                user.UserId = 99;
                return user;
            });

        var service = new AuthService(authRepository.Object, tokenGenerator.Object, jwtSettings, currentUserContext.Object);

        var response = await service.RegisterAsync(new RegisterRequestDto
        {
            EmployeeId = 10,
            Username = "rahul",
            Password = "rahul@1234"
        });

        createdUser.Should().NotBeNull();
        createdUser!.Role.Should().Be("Employee");
        response.Role.Should().Be("Employee");
    }

    [Fact]
    public async Task RegisterAsync_Throws_WhenEmployeeNotFound()
    {
        var authRepository = new Mock<IAuthRepository>();
        var tokenGenerator = new Mock<IJwtTokenGenerator>();
        var currentUserContext = new Mock<ICurrentUserContext>();
        var jwtSettings = Options.Create(new Application.Configuration.JwtSettings
        {
            Issuer = "issuer",
            Audience = "audience",
            Key = "super-secret-test-key-super-secret-test-key",
            ExpireMinutes = 60
        });

        authRepository.Setup(x => x.GetEmployeeByIdAsync(10))
            .ReturnsAsync((Employee?)null);

        var service = new AuthService(authRepository.Object, tokenGenerator.Object, jwtSettings, currentUserContext.Object);

        var act = async () => await service.RegisterAsync(new RegisterRequestDto
        {
            EmployeeId = 10,
            Username = "rahul",
            Password = "rahul@1234"
        });

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Employee not found.");
    }

    [Fact]
    public async Task RegisterAsync_Throws_WhenEmployeeIsInactive()
    {
        var authRepository = new Mock<IAuthRepository>();
        var tokenGenerator = new Mock<IJwtTokenGenerator>();
        var currentUserContext = new Mock<ICurrentUserContext>();
        var jwtSettings = Options.Create(new Application.Configuration.JwtSettings
        {
            Issuer = "issuer",
            Audience = "audience",
            Key = "super-secret-test-key-super-secret-test-key",
            ExpireMinutes = 60
        });

        authRepository.Setup(x => x.GetEmployeeByIdAsync(10))
            .ReturnsAsync(new Employee
            {
                EmployeeId = 10,
                IsActive = false
            });

        var service = new AuthService(authRepository.Object, tokenGenerator.Object, jwtSettings, currentUserContext.Object);

        var act = async () => await service.RegisterAsync(new RegisterRequestDto
        {
            EmployeeId = 10,
            Username = "rahul",
            Password = "rahul@1234"
        });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Employee is inactive.");
    }

    [Fact]
    public async Task RegisterAsync_Throws_WhenEmployeeAlreadyHasUserAccount()
    {
        var authRepository = new Mock<IAuthRepository>();
        var tokenGenerator = new Mock<IJwtTokenGenerator>();
        var currentUserContext = new Mock<ICurrentUserContext>();
        var jwtSettings = Options.Create(new Application.Configuration.JwtSettings
        {
            Issuer = "issuer",
            Audience = "audience",
            Key = "super-secret-test-key-super-secret-test-key",
            ExpireMinutes = 60
        });

        authRepository.Setup(x => x.GetEmployeeByIdAsync(10))
            .ReturnsAsync(new Employee
            {
                EmployeeId = 10,
                IsActive = true
            });
        authRepository.Setup(x => x.EmployeeAlreadyRegisteredAsync(10))
            .ReturnsAsync(true);

        var service = new AuthService(authRepository.Object, tokenGenerator.Object, jwtSettings, currentUserContext.Object);

        var act = async () => await service.RegisterAsync(new RegisterRequestDto
        {
            EmployeeId = 10,
            Username = "rahul",
            Password = "rahul@1234"
        });

        await act.Should().ThrowAsync<Application.Exceptions.EmployeeAlreadyRegisteredException>()
            .WithMessage("A user account already exists for this employee.");
    }

    [Fact]
    public async Task RegisterAsync_Throws_WhenUsernameAlreadyExists()
    {
        var authRepository = new Mock<IAuthRepository>();
        var tokenGenerator = new Mock<IJwtTokenGenerator>();
        var currentUserContext = new Mock<ICurrentUserContext>();
        var jwtSettings = Options.Create(new Application.Configuration.JwtSettings
        {
            Issuer = "issuer",
            Audience = "audience",
            Key = "super-secret-test-key-super-secret-test-key",
            ExpireMinutes = 60
        });

        authRepository.Setup(x => x.GetEmployeeByIdAsync(10))
            .ReturnsAsync(new Employee
            {
                EmployeeId = 10,
                IsActive = true
            });
        authRepository.Setup(x => x.EmployeeAlreadyRegisteredAsync(10))
            .ReturnsAsync(false);
        authRepository.Setup(x => x.UsernameExistsAsync("rahul"))
            .ReturnsAsync(true);

        var service = new AuthService(authRepository.Object, tokenGenerator.Object, jwtSettings, currentUserContext.Object);

        var act = async () => await service.RegisterAsync(new RegisterRequestDto
        {
            EmployeeId = 10,
            Username = "rahul",
            Password = "rahul@1234"
        });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Username already exists.");
    }

    [Fact]
    public async Task LoginAsync_Throws_DeactivatedMessage_ForInactiveUser()
    {
        var authRepository = new Mock<IAuthRepository>();
        var tokenGenerator = new Mock<IJwtTokenGenerator>();
        var currentUserContext = new Mock<ICurrentUserContext>();
        var jwtSettings = Options.Create(new Application.Configuration.JwtSettings
        {
            Issuer = "issuer",
            Audience = "audience",
            Key = "super-secret-test-key-super-secret-test-key",
            ExpireMinutes = 60
        });

        authRepository.Setup(x => x.GetUserByUsernameAsync("rahul"))
            .ReturnsAsync(new User
            {
                UserId = 1,
                EmployeeId = 10,
                Username = "rahul",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("rahul@1234"),
                Role = "Employee",
                IsActive = false
            });

        var service = new AuthService(authRepository.Object, tokenGenerator.Object, jwtSettings, currentUserContext.Object);

        var act = async () => await service.LoginAsync(new LoginRequestDto
        {
            Username = "rahul",
            Password = "rahul@1234"
        });

        await act.Should().ThrowAsync<Application.Exceptions.UnauthorizedException>()
            .WithMessage("Your account has been deactivated. Please contact an administrator.");
    }

    [Fact]
    public async Task LoginAsync_Throws_InvalidCredentials_BeforeInactiveCheck_WhenPasswordIsWrong()
    {
        var authRepository = new Mock<IAuthRepository>();
        var tokenGenerator = new Mock<IJwtTokenGenerator>();
        var currentUserContext = new Mock<ICurrentUserContext>();
        var jwtSettings = Options.Create(new Application.Configuration.JwtSettings
        {
            Issuer = "issuer",
            Audience = "audience",
            Key = "super-secret-test-key-super-secret-test-key",
            ExpireMinutes = 60
        });

        authRepository.Setup(x => x.GetUserByUsernameAsync("rahul"))
            .ReturnsAsync(new User
            {
                UserId = 1,
                EmployeeId = 10,
                Username = "rahul",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("rahul@1234"),
                Role = "Employee",
                IsActive = false
            });

        var service = new AuthService(authRepository.Object, tokenGenerator.Object, jwtSettings, currentUserContext.Object);

        var act = async () => await service.LoginAsync(new LoginRequestDto
        {
            Username = "rahul",
            Password = "wrong-password"
        });

        await act.Should().ThrowAsync<Application.Exceptions.UnauthorizedException>()
            .WithMessage("Invalid username or password");
    }
}

public class ChangePasswordServiceTests
{
    [Fact]
    public async Task ChangePasswordAsync_UpdatesPasswordHash_WhenRequestIsValid()
    {
        var authRepository = new Mock<IAuthRepository>();
        var tokenGenerator = new Mock<IJwtTokenGenerator>();
        var currentUserContext = new Mock<ICurrentUserContext>();
        currentUserContext.Setup(x => x.UserId).Returns(5);

        var currentPassword = "Old@123";
        var newPassword = "New@123";
        var currentPasswordHash = BCrypt.Net.BCrypt.HashPassword(currentPassword);

        authRepository.Setup(x => x.GetUserByIdAsync(5)).ReturnsAsync(new User
        {
            UserId = 5,
            EmployeeId = 10,
            Username = "rahul",
            PasswordHash = currentPasswordHash,
            Role = "Employee",
            IsActive = true
        });

        string? capturedNewPasswordHash = null;
        authRepository.Setup(x => x.ChangePasswordAsync(5, It.IsAny<string>()))
            .Callback<int, string>((_, hash) => capturedNewPasswordHash = hash)
            .Returns(Task.CompletedTask);

        var jwtSettings = Options.Create(new Application.Configuration.JwtSettings
        {
            Issuer = "issuer",
            Audience = "audience",
            Key = "super-secret-test-key-super-secret-test-key",
            ExpireMinutes = 60
        });

        var service = new AuthService(authRepository.Object, tokenGenerator.Object, jwtSettings, currentUserContext.Object);

        var response = await service.ChangePasswordAsync(new ChangePasswordDto
        {
            CurrentPassword = currentPassword,
            NewPassword = newPassword,
            ConfirmPassword = newPassword
        });

        response.Message.Should().Be("Password changed successfully.");
        capturedNewPasswordHash.Should().NotBeNull();
        capturedNewPasswordHash!.Should().NotBe(currentPasswordHash);
        BCrypt.Net.BCrypt.Verify(newPassword, capturedNewPasswordHash).Should().BeTrue();
        authRepository.Verify(x => x.ChangePasswordAsync(5, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_Throws_WhenConfirmationDoesNotMatch()
    {
        var authRepository = new Mock<IAuthRepository>();
        var tokenGenerator = new Mock<IJwtTokenGenerator>();
        var currentUserContext = new Mock<ICurrentUserContext>();
        currentUserContext.Setup(x => x.UserId).Returns(5);

        authRepository.Setup(x => x.GetUserByIdAsync(5)).ReturnsAsync(new User
        {
            UserId = 5,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Old@123"),
            Role = "Employee",
            IsActive = true
        });

        var jwtSettings = Options.Create(new Application.Configuration.JwtSettings
        {
            Issuer = "issuer",
            Audience = "audience",
            Key = "super-secret-test-key-super-secret-test-key",
            ExpireMinutes = 60
        });

        var service = new AuthService(authRepository.Object, tokenGenerator.Object, jwtSettings, currentUserContext.Object);

        var act = async () => await service.ChangePasswordAsync(new ChangePasswordDto
        {
            CurrentPassword = "Old@123",
            NewPassword = "New@123",
            ConfirmPassword = "Different@123"
        });

        await act.Should().ThrowAsync<Application.Exceptions.PasswordConfirmationMismatchException>()
            .WithMessage("New password and confirmation password do not match.");
        authRepository.Verify(x => x.ChangePasswordAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_Throws_WhenPasswordIsTooWeak()
    {
        var authRepository = new Mock<IAuthRepository>();
        var tokenGenerator = new Mock<IJwtTokenGenerator>();
        var currentUserContext = new Mock<ICurrentUserContext>();
        currentUserContext.Setup(x => x.UserId).Returns(5);

        authRepository.Setup(x => x.GetUserByIdAsync(5)).ReturnsAsync(new User
        {
            UserId = 5,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Old@123"),
            Role = "Employee",
            IsActive = true
        });

        var jwtSettings = Options.Create(new Application.Configuration.JwtSettings
        {
            Issuer = "issuer",
            Audience = "audience",
            Key = "super-secret-test-key-super-secret-test-key",
            ExpireMinutes = 60
        });

        var service = new AuthService(authRepository.Object, tokenGenerator.Object, jwtSettings, currentUserContext.Object);

        var act = async () => await service.ChangePasswordAsync(new ChangePasswordDto
        {
            CurrentPassword = "Old@123",
            NewPassword = "12345",
            ConfirmPassword = "12345"
        });

        await act.Should().ThrowAsync<Application.Exceptions.PasswordStrengthException>()
            .WithMessage("Password does not meet minimum strength requirements.");
        authRepository.Verify(x => x.ChangePasswordAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_Throws_WhenCurrentPasswordIsIncorrect()
    {
        var authRepository = new Mock<IAuthRepository>();
        var tokenGenerator = new Mock<IJwtTokenGenerator>();
        var currentUserContext = new Mock<ICurrentUserContext>();
        currentUserContext.Setup(x => x.UserId).Returns(5);

        authRepository.Setup(x => x.GetUserByIdAsync(5)).ReturnsAsync(new User
        {
            UserId = 5,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Old@123"),
            Role = "Employee",
            IsActive = true
        });

        var jwtSettings = Options.Create(new Application.Configuration.JwtSettings
        {
            Issuer = "issuer",
            Audience = "audience",
            Key = "super-secret-test-key-super-secret-test-key",
            ExpireMinutes = 60
        });

        var service = new AuthService(authRepository.Object, tokenGenerator.Object, jwtSettings, currentUserContext.Object);

        var act = async () => await service.ChangePasswordAsync(new ChangePasswordDto
        {
            CurrentPassword = "Wrong@123",
            NewPassword = "New@123",
            ConfirmPassword = "New@123"
        });

        await act.Should().ThrowAsync<Application.Exceptions.CurrentPasswordIncorrectException>()
            .WithMessage("Current password is incorrect.");
        authRepository.Verify(x => x.ChangePasswordAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_Throws_WhenNewPasswordMatchesCurrentPassword()
    {
        var authRepository = new Mock<IAuthRepository>();
        var tokenGenerator = new Mock<IJwtTokenGenerator>();
        var currentUserContext = new Mock<ICurrentUserContext>();
        currentUserContext.Setup(x => x.UserId).Returns(5);

        authRepository.Setup(x => x.GetUserByIdAsync(5)).ReturnsAsync(new User
        {
            UserId = 5,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Old@123"),
            Role = "Employee",
            IsActive = true
        });

        var jwtSettings = Options.Create(new Application.Configuration.JwtSettings
        {
            Issuer = "issuer",
            Audience = "audience",
            Key = "super-secret-test-key-super-secret-test-key",
            ExpireMinutes = 60
        });

        var service = new AuthService(authRepository.Object, tokenGenerator.Object, jwtSettings, currentUserContext.Object);

        var act = async () => await service.ChangePasswordAsync(new ChangePasswordDto
        {
            CurrentPassword = "Old@123",
            NewPassword = "Old@123",
            ConfirmPassword = "Old@123"
        });

        await act.Should().ThrowAsync<Application.Exceptions.NewPasswordMatchesCurrentPasswordException>()
            .WithMessage("New password cannot be the same as the current password.");
        authRepository.Verify(x => x.ChangePasswordAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_Throws_WhenUserIsInactive()
    {
        var authRepository = new Mock<IAuthRepository>();
        var tokenGenerator = new Mock<IJwtTokenGenerator>();
        var currentUserContext = new Mock<ICurrentUserContext>();
        currentUserContext.Setup(x => x.UserId).Returns(5);

        authRepository.Setup(x => x.GetUserByIdAsync(5)).ReturnsAsync(new User
        {
            UserId = 5,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Old@123"),
            Role = "Employee",
            IsActive = false
        });

        var jwtSettings = Options.Create(new Application.Configuration.JwtSettings
        {
            Issuer = "issuer",
            Audience = "audience",
            Key = "super-secret-test-key-super-secret-test-key",
            ExpireMinutes = 60
        });

        var service = new AuthService(authRepository.Object, tokenGenerator.Object, jwtSettings, currentUserContext.Object);

        var act = async () => await service.ChangePasswordAsync(new ChangePasswordDto
        {
            CurrentPassword = "Old@123",
            NewPassword = "New@123",
            ConfirmPassword = "New@123"
        });

        await act.Should().ThrowAsync<Application.Exceptions.AccountInactiveException>()
            .WithMessage("Your account is inactive.");
        authRepository.Verify(x => x.ChangePasswordAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_Throws_WhenUserDoesNotExist()
    {
        var authRepository = new Mock<IAuthRepository>();
        var tokenGenerator = new Mock<IJwtTokenGenerator>();
        var currentUserContext = new Mock<ICurrentUserContext>();
        currentUserContext.Setup(x => x.UserId).Returns(5);

        authRepository.Setup(x => x.GetUserByIdAsync(5)).ReturnsAsync((User?)null);

        var jwtSettings = Options.Create(new Application.Configuration.JwtSettings
        {
            Issuer = "issuer",
            Audience = "audience",
            Key = "super-secret-test-key-super-secret-test-key",
            ExpireMinutes = 60
        });

        var service = new AuthService(authRepository.Object, tokenGenerator.Object, jwtSettings, currentUserContext.Object);

        var act = async () => await service.ChangePasswordAsync(new ChangePasswordDto
        {
            CurrentPassword = "Old@123",
            NewPassword = "New@123",
            ConfirmPassword = "New@123"
        });

        await act.Should().ThrowAsync<Application.Exceptions.UserNotFoundException>()
            .WithMessage("User not found.");
    }

    [Fact]
    public async Task ChangePasswordAsync_Throws_WhenUserContextIsMissing()
    {
        var authRepository = new Mock<IAuthRepository>();
        var tokenGenerator = new Mock<IJwtTokenGenerator>();
        var currentUserContext = new Mock<ICurrentUserContext>();
        currentUserContext.Setup(x => x.UserId).Returns((int?)null);

        var jwtSettings = Options.Create(new Application.Configuration.JwtSettings
        {
            Issuer = "issuer",
            Audience = "audience",
            Key = "super-secret-test-key-super-secret-test-key",
            ExpireMinutes = 60
        });

        var service = new AuthService(authRepository.Object, tokenGenerator.Object, jwtSettings, currentUserContext.Object);

        var act = async () => await service.ChangePasswordAsync(new ChangePasswordDto
        {
            CurrentPassword = "Old@123",
            NewPassword = "New@123",
            ConfirmPassword = "New@123"
        });

        await act.Should().ThrowAsync<Application.Exceptions.UnauthorizedException>();
    }
}

public class PerformanceReviewServiceTests
{
    [Fact]
    public async Task GetAllAsync_ForEmployee_UsesEmployeeScopedRepository()
    {
        var reviews = new[]
        {
            new PerformanceReview
            {
                PerformanceReviewId = 1,
                ReviewCycleId = 1,
                EmployeeId = 7,
                ManagerId = 2,
                Status = "Draft",
                CreatedAt = DateTime.UtcNow
            }
        };

        var repository = new Mock<IPerformanceReviewRepository>();
        repository.Setup(x => x.GetByEmployeeIdAsync(7)).ReturnsAsync(reviews);

        var ratingRepository = new Mock<IRatingRepository>();
        var service = new PerformanceReviewService(repository.Object, ratingRepository.Object);

        var result = await service.GetAllAsync(new CurrentUserContextDto
        {
            EmployeeId = 7,
            Role = "Employee"
        });

        result.Should().HaveCount(1);
        result.Single().EmployeeId.Should().Be(7);
        repository.Verify(x => x.GetByEmployeeIdAsync(7), Times.Once);
        repository.Verify(x => x.GetAllAsync(), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_ForEmployee_DoesNotReturnOtherEmployeesReview()
    {
        var repository = new Mock<IPerformanceReviewRepository>();
        repository.Setup(x => x.GetByIdForEmployeeAsync(5, 7)).ReturnsAsync((PerformanceReview?)null);

        var ratingRepository = new Mock<IRatingRepository>();
        var service = new PerformanceReviewService(repository.Object, ratingRepository.Object);

        var result = await service.GetByIdAsync(5, new CurrentUserContextDto
        {
            EmployeeId = 7,
            Role = "Employee"
        });

        result.Should().BeNull();
        repository.Verify(x => x.GetByIdForEmployeeAsync(5, 7), Times.Once);
        repository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task SubmitSelfAssessmentAsync_UpdatesOnlyEmployeeOwnedDraftReview()
    {
        var review = new PerformanceReview
        {
            PerformanceReviewId = 15,
            ReviewCycleId = 3,
            EmployeeId = 7,
            ManagerId = 2,
            SelfAssessment = null,
            ManagerComments = "Keep",
            OverallRating = 4.5m,
            Status = "Draft",
            ApprovedDate = null,
            CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc),
            ModifiedAt = null,
            ReviewCycle = new ReviewCycle
            {
                ReviewCycleId = 3,
                Status = "Active"
            }
        };

        var repository = new Mock<IPerformanceReviewRepository>();
        repository.Setup(x => x.GetPerformanceReviewByIdAsync(15)).ReturnsAsync(review);
        repository.Setup(x => x.SubmitSelfAssessmentAsync(review)).Returns(Task.CompletedTask);

        var ratingRepository = new Mock<IRatingRepository>();
        var service = new PerformanceReviewService(repository.Object, ratingRepository.Object);
        var nowBefore = DateTime.UtcNow;

        await service.SubmitSelfAssessmentAsync(15, new SubmitSelfAssessmentDto
        {
            SelfAssessment = "I completed all assigned work and met project deadlines."
        }, new CurrentUserContextDto
        {
            EmployeeId = 7,
            Role = "Employee"
        });

        review.SelfAssessment.Should().Be("I completed all assigned work and met project deadlines.");
        review.Status.Should().Be("Submitted");
        review.SubmittedDate.Should().NotBeNull();
        review.SubmittedDate.Should().BeOnOrAfter(nowBefore);
        review.ModifiedAt.Should().NotBeNull();
        review.ManagerComments.Should().Be("Keep");
        review.OverallRating.Should().Be(4.5m);
        review.ApprovedDate.Should().BeNull();
        repository.Verify(x => x.SubmitSelfAssessmentAsync(review), Times.Once);
    }

    [Fact]
    public async Task SubmitSelfAssessmentAsync_Throws_WhenReviewDoesNotExist()
    {
        var repository = new Mock<IPerformanceReviewRepository>();
        repository.Setup(x => x.GetPerformanceReviewByIdAsync(15)).ReturnsAsync((PerformanceReview?)null);

        var ratingRepository = new Mock<IRatingRepository>();
        var service = new PerformanceReviewService(repository.Object, ratingRepository.Object);

        var act = async () => await service.SubmitSelfAssessmentAsync(15, new SubmitSelfAssessmentDto
        {
            SelfAssessment = "Complete"
        }, new CurrentUserContextDto
        {
            EmployeeId = 7,
            Role = "Employee"
        });

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Performance Review not found.");
    }

    [Fact]
    public async Task SubmitSelfAssessmentAsync_Throws_WhenEmployeeDoesNotOwnReview()
    {
        var repository = new Mock<IPerformanceReviewRepository>();
        repository.Setup(x => x.GetPerformanceReviewByIdAsync(15)).ReturnsAsync(new PerformanceReview
        {
            PerformanceReviewId = 15,
            ReviewCycleId = 3,
            EmployeeId = 9,
            ManagerId = 2,
            Status = "Draft",
            ReviewCycle = new ReviewCycle { ReviewCycleId = 3, Status = "Active" }
        });

        var ratingRepository = new Mock<IRatingRepository>();
        var service = new PerformanceReviewService(repository.Object, ratingRepository.Object);

        var act = async () => await service.SubmitSelfAssessmentAsync(15, new SubmitSelfAssessmentDto
        {
            SelfAssessment = "Complete"
        }, new CurrentUserContextDto
        {
            EmployeeId = 7,
            Role = "Employee"
        });

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("You are not authorized to update this review.");
    }

    [Fact]
    public async Task SubmitSelfAssessmentAsync_Throws_WhenCycleIsClosed()
    {
        var repository = new Mock<IPerformanceReviewRepository>();
        repository.Setup(x => x.GetPerformanceReviewByIdAsync(15)).ReturnsAsync(new PerformanceReview
        {
            PerformanceReviewId = 15,
            ReviewCycleId = 3,
            EmployeeId = 7,
            ManagerId = 2,
            Status = "Draft",
            ReviewCycle = new ReviewCycle { ReviewCycleId = 3, Status = "Closed" }
        });

        var ratingRepository = new Mock<IRatingRepository>();
        var service = new PerformanceReviewService(repository.Object, ratingRepository.Object);

        var act = async () => await service.SubmitSelfAssessmentAsync(15, new SubmitSelfAssessmentDto
        {
            SelfAssessment = "Complete"
        }, new CurrentUserContextDto
        {
            EmployeeId = 7,
            Role = "Employee"
        });

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("The review cycle is no longer accepting self assessments.");
    }

    [Fact]
    public async Task SubmitSelfAssessmentAsync_Throws_WhenAlreadySubmitted()
    {
        var repository = new Mock<IPerformanceReviewRepository>();
        repository.Setup(x => x.GetPerformanceReviewByIdAsync(15)).ReturnsAsync(new PerformanceReview
        {
            PerformanceReviewId = 15,
            ReviewCycleId = 3,
            EmployeeId = 7,
            ManagerId = 2,
            SelfAssessment = "Existing",
            Status = "Submitted",
            ReviewCycle = new ReviewCycle { ReviewCycleId = 3, Status = "Active" }
        });

        var ratingRepository = new Mock<IRatingRepository>();
        var service = new PerformanceReviewService(repository.Object, ratingRepository.Object);

        var act = async () => await service.SubmitSelfAssessmentAsync(15, new SubmitSelfAssessmentDto
        {
            SelfAssessment = "Complete"
        }, new CurrentUserContextDto
        {
            EmployeeId = 7,
            Role = "Employee"
        });

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Cannot submit self-assessment in the current status.");
    }

    [Fact]
    public async Task SubmitSelfAssessmentAsync_Throws_WhenAssessmentMissing()
    {
        var repository = new Mock<IPerformanceReviewRepository>();
        repository.Setup(x => x.GetPerformanceReviewByIdAsync(15)).ReturnsAsync(new PerformanceReview
        {
            PerformanceReviewId = 15,
            ReviewCycleId = 3,
            EmployeeId = 7,
            ManagerId = 2,
            Status = "Draft",
            ReviewCycle = new ReviewCycle { ReviewCycleId = 3, Status = "Active" }
        });

        var ratingRepository = new Mock<IRatingRepository>();
        var service = new PerformanceReviewService(repository.Object, ratingRepository.Object);

        var act = async () => await service.SubmitSelfAssessmentAsync(15, new SubmitSelfAssessmentDto
        {
            SelfAssessment = "   "
        }, new CurrentUserContextDto
        {
            EmployeeId = 7,
            Role = "Employee"
        });

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Self assessment is required.");
    }

    [Fact]
    public async Task AddAsync_Throws_WhenDuplicatePerformanceReviewExists()
    {
        var repository = new Mock<IPerformanceReviewRepository>();
        repository.Setup(x => x.ExistsByEmployeeAndCycleAsync(7, 3)).ReturnsAsync(true);

        var ratingRepository = new Mock<IRatingRepository>();
        var service = new PerformanceReviewService(repository.Object, ratingRepository.Object);

        var act = async () => await service.AddAsync(new CreatePerformanceReviewDto
        {
            ReviewCycleId = 3,
            EmployeeId = 7,
            ManagerId = 2
        });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("A performance review already exists for this employee in this review cycle.");
        repository.Verify(x => x.AddAsync(It.IsAny<PerformanceReview>()), Times.Never);
    }

    [Fact]
    public async Task ManagerReviewAsync_Approves_SubmittedReview()
    {
        var review = new PerformanceReview
        {
            PerformanceReviewId = 21,
            ReviewCycleId = 3,
            EmployeeId = 7,
            ManagerId = 2,
            Status = "Submitted",
            SelfAssessment = "Great progress",
            ReviewCycle = new ReviewCycle { ReviewCycleId = 3, Status = "Active" }
        };

        var repository = new Mock<IPerformanceReviewRepository>();
        repository.Setup(x => x.GetPerformanceReviewByIdAsync(21)).ReturnsAsync(review);
        repository.Setup(x => x.UpdateManagerReviewAsync(review)).Returns(Task.CompletedTask);
        var ratingRepository = new Mock<IRatingRepository>();
        ratingRepository.Setup(x => x.GetByPerformanceReviewIdAsync(21)).ReturnsAsync(new[]
        {
            new Rating { RatingId = 1, PerformanceReviewId = 21, Criteria = "Coding", Score = 5, CreatedAt = DateTime.UtcNow },
            new Rating { RatingId = 2, PerformanceReviewId = 21, Criteria = "Communication", Score = 4, CreatedAt = DateTime.UtcNow },
            new Rating { RatingId = 3, PerformanceReviewId = 21, Criteria = "Leadership", Score = 5, CreatedAt = DateTime.UtcNow }
        });

        var service = new PerformanceReviewService(repository.Object, ratingRepository.Object);

        await service.ManagerReviewAsync(21, new ManagerReviewDto
        {
            Action = "Approve",
            ManagerComments = "Excellent ownership and delivery."
        }, new CurrentUserContextDto
        {
            EmployeeId = 2,
            Role = "Manager"
        });

        review.ManagerComments.Should().Be("Excellent ownership and delivery.");
        review.OverallRating.Should().Be(4.67m);
        review.Status.Should().Be("Approved");
        review.ApprovedDate.Should().NotBeNull();
        review.ModifiedAt.Should().NotBeNull();
        repository.Verify(x => x.UpdateManagerReviewAsync(review), Times.Once);
    }

    [Fact]
    public async Task ManagerReviewAsync_NeedsRevision_DoesNotChangeOverallRating()
    {
        var review = new PerformanceReview
        {
            PerformanceReviewId = 22,
            ReviewCycleId = 3,
            EmployeeId = 7,
            ManagerId = 2,
            Status = "Submitted",
            OverallRating = 3.5m,
            ReviewCycle = new ReviewCycle { ReviewCycleId = 3, Status = "Active" }
        };

        var repository = new Mock<IPerformanceReviewRepository>();
        repository.Setup(x => x.GetPerformanceReviewByIdAsync(22)).ReturnsAsync(review);
        repository.Setup(x => x.UpdateManagerReviewAsync(review)).Returns(Task.CompletedTask);
        var ratingRepository = new Mock<IRatingRepository>();

        var service = new PerformanceReviewService(repository.Object, ratingRepository.Object);

        await service.ManagerReviewAsync(22, new ManagerReviewDto
        {
            Action = "NeedsRevision",
            ManagerComments = "Please improve documentation."
        }, new CurrentUserContextDto
        {
            EmployeeId = 2,
            Role = "Manager"
        });

        review.ManagerComments.Should().Be("Please improve documentation.");
        review.OverallRating.Should().Be(3.5m);
        review.Status.Should().Be("NeedsRevision");
        review.ApprovedDate.Should().BeNull();
        repository.Verify(x => x.UpdateManagerReviewAsync(review), Times.Once);
    }

    [Fact]
    public async Task ManagerReviewAsync_RejectsInvalidActionValue()
    {
        var repository = new Mock<IPerformanceReviewRepository>();
        var ratingRepository = new Mock<IRatingRepository>();
        var service = new PerformanceReviewService(repository.Object, ratingRepository.Object);

        var act = async () => await service.ManagerReviewAsync(21, new ManagerReviewDto
        {
            Action = "Reject",
            ManagerComments = "No"
        }, new CurrentUserContextDto
        {
            EmployeeId = 2,
            Role = "Manager"
        });

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Invalid action value. Allowed: Approve, NeedsRevision.");
    }
}
