using EmployeePerformance.Application.Exceptions;
using EmployeePerformance.Application.Interfaces;
using EmployeePerformance.Application.Services;
using EmployeePerformance.Domain.Entities;
using EmployeePerformance.Application.DTOs.UserManagement;
using FluentAssertions;
using Moq;

namespace EmployeePerformance.Tests;

public class UserManagementServiceTests
{
    [Fact]
    public async Task UpdateUserRoleAsync_UpdatesRole_WhenRequestIsValid()
    {
        var repository = new Mock<IUserManagementRepository>();
        repository.Setup(x => x.GetUserEntityByIdAsync(5)).ReturnsAsync(new User
        {
            UserId = 5,
            Username = "Rahul",
            Role = "Employee"
        });

        repository.Setup(x => x.UpdateRoleAsync(5, "Manager")).Returns(Task.CompletedTask);

        var service = new UserManagementService(repository.Object);

        var response = await service.UpdateUserRoleAsync(5, "Manager", authenticatedUserId: 1);

        response.UserId.Should().Be(5);
        response.Username.Should().Be("Rahul");
        response.OldRole.Should().Be("Employee");
        response.NewRole.Should().Be("Manager");
        response.Message.Should().Be("User role updated successfully.");
        repository.Verify(x => x.UpdateRoleAsync(5, "Manager"), Times.Once);
    }

    [Fact]
    public async Task UpdateUserRoleAsync_Throws_WhenRoleIsInvalid()
    {
        var repository = new Mock<IUserManagementRepository>();
        var service = new UserManagementService(repository.Object);

        var act = async () => await service.UpdateUserRoleAsync(5, "manager", authenticatedUserId: 1);

        await act.Should().ThrowAsync<InvalidRoleException>()
            .WithMessage("Invalid role.");
    }

    [Fact]
    public async Task UpdateUserRoleAsync_Throws_WhenChangingOwnRole()
    {
        var repository = new Mock<IUserManagementRepository>();
        var service = new UserManagementService(repository.Object);

        var act = async () => await service.UpdateUserRoleAsync(5, "Manager", authenticatedUserId: 5);

        await act.Should().ThrowAsync<CannotChangeOwnRoleException>()
            .WithMessage("Cannot change your own role.");
    }

    [Fact]
    public async Task UpdateUserRoleAsync_Throws_WhenRemovingLastAdmin()
    {
        var repository = new Mock<IUserManagementRepository>();
        repository.Setup(x => x.GetUserEntityByIdAsync(5)).ReturnsAsync(new User
        {
            UserId = 5,
            Username = "AdminUser",
            Role = "Admin"
        });
        repository.Setup(x => x.CountUsersByRoleAsync("Admin")).ReturnsAsync(1);

        var service = new UserManagementService(repository.Object);

        var act = async () => await service.UpdateUserRoleAsync(5, "Manager", authenticatedUserId: 1);

        await act.Should().ThrowAsync<CannotRemoveLastAdminException>()
            .WithMessage("Cannot remove the last remaining Admin.");
        repository.Verify(x => x.UpdateRoleAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUserRoleAsync_Throws_WhenUserDoesNotExist()
    {
        var repository = new Mock<IUserManagementRepository>();
        repository.Setup(x => x.GetUserEntityByIdAsync(5)).ReturnsAsync((User?)null);

        var service = new UserManagementService(repository.Object);

        var act = async () => await service.UpdateUserRoleAsync(5, "Manager", authenticatedUserId: 1);

        await act.Should().ThrowAsync<UserNotFoundException>()
            .WithMessage("User not found.");
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsUserDetails_WhenUserExists()
    {
        var repository = new Mock<IUserManagementRepository>();
        repository.Setup(x => x.GetUserByIdAsync(5)).ReturnsAsync(new UserDetailsDto
        {
            UserId = 5,
            EmployeeId = 11,
            Username = "Rahul",
            Role = "Admin",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        var service = new UserManagementService(repository.Object);

        var result = await service.GetUserByIdAsync(5);

        result.Should().NotBeNull();
        result!.UserId.Should().Be(5);
        result.Username.Should().Be("Rahul");
        repository.Verify(x => x.GetUserByIdAsync(5), Times.Once);
    }

    [Fact]
    public async Task GetUsersAsync_ReturnsPagedResult_WhenQueryIsValid()
    {
        var repository = new Mock<IUserManagementRepository>();
        repository.Setup(x => x.GetUsersAsync(It.IsAny<UserQueryParametersDto>()))
            .ReturnsAsync(new EmployeePerformance.Application.DTOs.Common.PagedResultDto<UserListDto>
            {
                PageNumber = 1,
                PageSize = 10,
                TotalRecords = 1,
                TotalPages = 1,
                Data = new[]
                {
                    new UserListDto
                    {
                        UserId = 5,
                        EmployeeId = 11,
                        Username = "Rahul",
                        Role = "Admin",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                }
            });

        var service = new UserManagementService(repository.Object);

        var result = await service.GetUsersAsync(new UserQueryParametersDto());

        result.TotalRecords.Should().Be(1);
        result.Data.Should().ContainSingle();
        repository.Verify(x => x.GetUsersAsync(It.IsAny<UserQueryParametersDto>()), Times.Once);
    }
}
