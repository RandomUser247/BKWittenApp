using Xunit;
using Moq;
using BackendServer.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ContentDB; // Namespace anpassen, falls nötig
using System.Collections.Generic;

public class BkwControllerTests
{
    private BkwController _controller;
    private ContentDBContext _context;

    public BkwControllerTests()
    {
        var options = new DbContextOptionsBuilder<ContentDBContext>()
            .UseInMemoryDatabase(databaseName: "TestDB")
            .Options;

        _context = new ContentDBContext(options);

        // Daten vorbereiten
        _context.Users.AddRange(
            new User { UserID = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" },
            new User { UserID = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com" }
        );
        _context.SaveChanges();

        _controller = new BkwController(_context);
    }

    [Fact]
    public async Task GetUsers_ReturnsOkResult_WithListOfUsers()
    {
        // Act
        var result = await _controller.GetUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var users = Assert.IsType<List<User>>(okResult.Value);
        Assert.Equal(2, users.Count);
    }

    [Fact]
    public async Task GetUserById_ReturnsOkResult_WhenUserExists()
    {
        // Act
        var result = await _controller.GetUserById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var user = Assert.IsType<User>(okResult.Value);
        Assert.Equal(1, user.UserID);
    }

    [Fact]
    public async Task GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Act
        var result = await _controller.GetUserById(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task CreateUser_ReturnsCreatedAtAction_WhenValidUser()
    {
        // Arrange
        var newUser = new User { UserID = 3, FirstName = "Alice", LastName = "Johnson", Email = "alice.johnson@example.com" };

        // Act
        var result = await _controller.CreateUser(newUser);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var user = Assert.IsType<User>(createdAtActionResult.Value);
        Assert.Equal(3, user.UserID);
    }

    [Fact]
    public async Task UpdateUser_ReturnsOkResult_WhenUserUpdatedSuccessfully()
    {
        // Arrange
        var updatedUser = new User { UserID = 1, FirstName = "Updated", LastName = "Doe", Email = "updated.doe@example.com" };

        // Act
        var result = await _controller.UpdateUser(1, updatedUser);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("User with ID 1 updated successfully.", (okResult.Value as dynamic)?.message);
    }

    [Fact]
    public async Task DeleteUser_ReturnsOkResult_WhenUserDeletedSuccessfully()
    {
        // Act
        var result = await _controller.DeleteUser(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("User with ID 1 deleted successfully.", (okResult.Value as dynamic)?.message);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Act
        var result = await _controller.DeleteUser(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
