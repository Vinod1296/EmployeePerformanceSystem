using EmployeePerformance.Application.Exceptions;
using EmployeePerformance.Application.Interfaces;
using EmployeePerformance.Application.Services;
using EmployeePerformance.Domain.Entities;
using FluentAssertions;
using Moq;

namespace EmployeePerformance.Tests;

public class UserStatusManagementServiceTests
{
    [Fact]
    public async Task UpdateUserStatusAsync_ActivatesUser_WhenRequestIsValid()
    {
        var repository = new Mock<IUserManagementRepository>();
        var user = new User
        {
            UserId = 5,
            Username = "Rahul",
            Role = "Employee",
            IsActive = false
        };
        repository.Setup(x => x.GetUserEntityByIdAsync(5)).ReturnsAsync(user);
        repository.Setup(x => x.UpdateUserStatusAsync(5, true))
            .Callback(() => user.IsActive = true)
            .Returns(Task.CompletedTask);

        var service = new UserManagementService(repository.Object);

        var response = await service.UpdateUserStatusAsync(5, true, authenticatedUserId: 1);

        response.UserId.Should().Be(5);
        response.Username.Should().Be("Rahul");
        response.OldStatus.Should().BeFalse();
        response.NewStatus.Should().BeTrue();
        response.Message.Should().Be("User status updated successfully.");
        repository.Verify(x => x.UpdateUserStatusAsync(5, true), Times.Once);
    }

    [Fact]
    public async Task UpdateUserStatusAsync_Throws_WhenUserAlreadyHasRequestedStatus()
    {
        var repository = new Mock<IUserManagementRepository>();
        repository.Setup(x => x.GetUserEntityByIdAsync(5)).ReturnsAsync(new User
        {
            UserId = 5,
            Username = "Rahul",
            Role = "Employee",
            IsActive = true
        });

        var service = new UserManagementService(repository.Object);

        var act = async () => await service.UpdateUserStatusAsync(5, true, authenticatedUserId: 1);

        await act.Should().ThrowAsync<UserStatusAlreadyUpdatedException>()
            .WithMessage("User status is already updated.");
    }

    [Fact]
    public async Task UpdateUserStatusAsync_Throws_WhenSelfDeactivating()
    {
        var repository = new Mock<IUserManagementRepository>();
        repository.Setup(x => x.GetUserEntityByIdAsync(5)).ReturnsAsync(new User
        {
            UserId = 5,
            Username = "Rahul",
            Role = "Admin",
            IsActive = true
        });

        var service = new UserManagementService(repository.Object);

        var act = async () => await service.UpdateUserStatusAsync(5, false, authenticatedUserId: 5);

        await act.Should().ThrowAsync<CannotDeactivateOwnAccountException>()
            .WithMessage("Cannot deactivate your own account.");
    }

    [Fact]
    public async Task UpdateUserStatusAsync_Throws_WhenDeactivatingLastActiveAdmin()
    {
        var repository = new Mock<IUserManagementRepository>();
        repository.Setup(x => x.GetUserEntityByIdAsync(5)).ReturnsAsync(new User
        {
            UserId = 5,
            Username = "AdminUser",
            Role = "Admin",
            IsActive = true
        });
        repository.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
            .Returns((Func<Task> operation) => operation());
        repository.Setup(x => x.CountActiveAdminsAsync()).ReturnsAsync(1);

        var service = new UserManagementService(repository.Object);

        var act = async () => await service.UpdateUserStatusAsync(5, false, authenticatedUserId: 1);

        await act.Should().ThrowAsync<CannotDeactivateLastActiveAdminException>()
            .WithMessage("Cannot deactivate the last active Admin.");
        repository.Verify(x => x.UpdateUserStatusAsync(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUserStatusAsync_DeactivatesAdmin_WhenMoreThanOneActiveAdminExists()
    {
        var repository = new Mock<IUserManagementRepository>();
        repository.Setup(x => x.GetUserEntityByIdAsync(5)).ReturnsAsync(new User
        {
            UserId = 5,
            Username = "AdminUser",
            Role = "Admin",
            IsActive = true
        });
        repository.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
            .Returns((Func<Task> operation) => operation());
        repository.Setup(x => x.CountActiveAdminsAsync()).ReturnsAsync(2);
        repository.Setup(x => x.UpdateUserStatusAsync(5, false)).Returns(Task.CompletedTask);

        var service = new UserManagementService(repository.Object);

        var response = await service.UpdateUserStatusAsync(5, false, authenticatedUserId: 1);

        response.UserId.Should().Be(5);
        response.OldStatus.Should().BeTrue();
        response.NewStatus.Should().BeFalse();
        repository.Verify(x => x.CountActiveAdminsAsync(), Times.Once);
        repository.Verify(x => x.UpdateUserStatusAsync(5, false), Times.Once);
    }

    [Fact]
    public async Task UpdateUserStatusAsync_Throws_WhenUserDoesNotExist()
    {
        var repository = new Mock<IUserManagementRepository>();
        repository.Setup(x => x.GetUserEntityByIdAsync(5)).ReturnsAsync((User?)null);

        var service = new UserManagementService(repository.Object);

        var act = async () => await service.UpdateUserStatusAsync(5, false, authenticatedUserId: 1);

        await act.Should().ThrowAsync<UserNotFoundException>()
            .WithMessage("User not found.");
    }
}
