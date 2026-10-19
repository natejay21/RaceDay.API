using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using Xunit;

namespace RaceDay.Tests.Controllers
{
    public class AuthControllerTests
    {
        // Helper method to create a fake in-memory database for testing
        private RaceDayDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name so tests don't clash
                .Options;
            return new RaceDayDbContext(options);
        }

        [Fact]
        public async Task Register_ValidUser_ReturnsOk()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var controller = new AuthController(context, null); // Config is null, we don't need it for register
            var dto = new RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@test.com",
                Password = "Password123!",
                Role = "Participant"
            };

            // Act
            var result = await controller.Register(dto);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var controller = new AuthController(context, null);

            // First, register a user
            await controller.Register(new RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@test.com",
                Password = "Password123!",
                Role = "Participant"
            });

            // Act: Try to login with wrong password
            var result = await controller.Login(new LoginDto
            {
                Email = "test@test.com",
                Password = "WrongPassword!"
            });

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }
    }
}